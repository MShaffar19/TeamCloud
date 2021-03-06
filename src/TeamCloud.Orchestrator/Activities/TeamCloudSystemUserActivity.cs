/**
 *  Copyright (c) Microsoft Corporation.
 *  Licensed under the MIT License.
 */

using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using TeamCloud.Azure;
using TeamCloud.Model.Data;

namespace TeamCloud.Orchestrator.Activities
{
    public class TeamCloudSystemUserActivity
    {
        private readonly IAzureSessionService azureSessionService;

        public TeamCloudSystemUserActivity(IAzureSessionService azureSessionService)
        {
            this.azureSessionService = azureSessionService ?? throw new System.ArgumentNullException(nameof(azureSessionService));
        }

        [FunctionName(nameof(TeamCloudSystemUserActivity))]
        public async Task<UserDocument> RunActivity(
            [ActivityTrigger] IDurableActivityContext activityContext)
        {
            if (activityContext is null)
                throw new System.ArgumentNullException(nameof(activityContext));

            var systemIdentity = await azureSessionService
                .GetIdentityAsync()
                .ConfigureAwait(false);

            return new UserDocument()
            {
                Id = systemIdentity.ObjectId.ToString(),
                Role = TeamCloudUserRole.None,
                UserType = UserType.System
            };
        }
    }
}
