/**
 *  Copyright (c) Microsoft Corporation.
 *  Licensed under the MIT License.
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.AzureAD.UI;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using TeamCloud.Configuration;
using TeamCloud.Data;
using TeamCloud.Data.Cosmos;
using TeamCloud.Model;

namespace TeamCloud.API
{

    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            app
                .UseHttpsRedirection()
                .UseRouting()
                .UseAuthentication()
                .UseAuthorization()
                .UseEndpoints(endpoints => endpoints.MapControllers());
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services
                .AddOptions(Assembly.GetExecutingAssembly());

            services
                .AddSingleton<IHttpContextAccessor, HttpContextAccessor>()
                .AddSingleton<Orchestrator>()
                .AddScoped<IProjectsRepositoryReadOnly, ProjectsRepository>()
                .AddScoped<ITeamCloudRepository, TeamCloudRepository>();

            ConfigureAuthentication(services);
            ConfigureAuthorization(services);

            services
                .AddControllers()
                .AddNewtonsoftJson();
        }

        private void ConfigureAuthentication(IServiceCollection services)
        {
            const string AzureAdSectionName = "AzureAD";

            services
                .AddAuthentication(AzureADDefaults.JwtBearerAuthenticationScheme)
                .AddAzureADBearer(options => Configuration.Bind(AzureAdSectionName, options));

            services
                .AddHttpContextAccessor()
                .Configure<AzureADOptions>(options => Configuration.Bind(AzureAdSectionName, options))
                .Configure<JwtBearerOptions>(AzureADDefaults.JwtBearerAuthenticationScheme, options =>
                {
                    // This is an Microsoft identity platform Web API
                    options.Authority += "/v2.0";

                    // Disable audience validation
                    options.TokenValidationParameters.ValidateAudience = false;

                    // Get the tenant ID from configuration to configure issuer validation
                    var tenantId = Configuration.GetSection(AzureAdSectionName).GetValue<string>("TenantId");

                    // The valid issuers can be based on Azure identity V1 or V2
                    options.TokenValidationParameters.ValidIssuers = new string[]
                    {
                        $"https://login.microsoftonline.com/{tenantId}/v2.0",
                        $"https://sts.windows.net/{tenantId}/"
                    };

                    options.Events = new JwtBearerEvents()
                    {
                        OnTokenValidated = async (TokenValidatedContext context) =>
                        {
                            var userId = context.Principal.GetObjectId();

                            var userClaims = await ResolveClaimsAsync(userId, context.HttpContext).ConfigureAwait(false);
                            if (userClaims.Any()) context.Principal.AddIdentity(new ClaimsIdentity(userClaims));
                        }
                    };
                });

        }

        private void ConfigureAuthorization(IServiceCollection services)
        {
            services
                .AddMvc(options =>
                {
                    // Requires authentication across the API
                    options.Filters.Add(new AuthorizeFilter("default"));
                });

            services
                .AddAuthorization(options =>
                {
                    options.AddPolicy("default", policy =>
                    {
                        policy.RequireAuthenticatedUser();
                    });

                    options.AddPolicy("admin", policy =>
                    {
                        policy.RequireRole(UserRoles.TeamCloud.Admin);
                    });

                    options.AddPolicy("projectCreate", policy =>
                    {
                        policy.RequireRole(UserRoles.TeamCloud.Admin, UserRoles.TeamCloud.Creator);
                    });

                    options.AddPolicy("projectRead", policy =>
                    {
                        policy.RequireRole(UserRoles.TeamCloud.Admin, UserRoles.Project.Owner, UserRoles.Project.Member);
                    });

                    options.AddPolicy("projectDelete", policy =>
                    {
                        policy.RequireRole(UserRoles.TeamCloud.Admin, UserRoles.Project.Owner);
                    });
                });
        }

        private async Task<IEnumerable<Claim>> ResolveClaimsAsync(Guid userId, HttpContext httpContext)
        {
            var claims = new List<Claim>()
            {
                // just for testing - everyone is an admin
                new Claim(ClaimTypes.Role, UserRoles.TeamCloud.Admin)
            };

            var projectIdRouteValue = httpContext.GetRouteData()
                .Values.GetValueOrDefault("ProjectId", StringComparison.OrdinalIgnoreCase)?.ToString();

            if (Guid.TryParse(projectIdRouteValue, out Guid projectId))
            {
                var projectRepository = httpContext.RequestServices
                    .GetRequiredService<IProjectsRepository>();

                var project = await projectRepository
                    .GetAsync(projectId)
                    .ConfigureAwait(false);

                var projectClaims = (project.Users ?? Enumerable.Empty<User>())
                    .Where(user => user.Id == userId)
                    .Select(user => new Claim(ClaimTypes.Role, user.Role));

                claims.AddRange(projectClaims);
            }

            return claims;
        }
    }
}