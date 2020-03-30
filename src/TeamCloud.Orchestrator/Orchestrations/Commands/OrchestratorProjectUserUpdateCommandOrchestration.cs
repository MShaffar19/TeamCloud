﻿/**
 *  Copyright (c) Microsoft Corporation.
 *  Licensed under the MIT License.
 */

using System;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Extensions.Logging;
using TeamCloud.Model.Commands;
using TeamCloud.Model.Data;
using TeamCloud.Orchestrator.Orchestrations.Commands.Activities;
using TeamCloud.Orchestrator.Orchestrations.Utilities;

namespace TeamCloud.Orchestrator.Orchestrations.Commands
{
    public static class OrchestratorProjectUserUpdateCommandOrchestration
    {
        [FunctionName(nameof(OrchestratorProjectUserUpdateCommandOrchestration))]
        public static async Task RunOrchestration(
            [OrchestrationTrigger] IDurableOrchestrationContext functionContext,
            ILogger log)
        {
            if (functionContext is null)
                throw new ArgumentNullException(nameof(functionContext));

            var commandMessage = functionContext.GetInput<OrchestratorCommandMessage>();
            var command = (OrchestratorProjectUserUpdateCommand)commandMessage.Command;
            var commandResult = command.CreateResult();
            var commandProject = default(Project);

            try
            {
                functionContext.SetCustomStatus($"Deleting user", log);

                using (await functionContext.LockAsync<Project>(command.ProjectId.ToString()).ConfigureAwait(true))
                {
                    commandProject = await functionContext
                        .GetProjectAsync(command.ProjectId.GetValueOrDefault())
                        .ConfigureAwait(true);

                    if (commandProject.Users.Remove(command.Payload))
                    {
                        commandProject = await functionContext
                            .SetProjectAsync(commandProject)
                            .ConfigureAwait(true);
                    }
                }

                functionContext.SetCustomStatus("Sending commands", log);

                var providerCommand = new ProviderProjectUserUpdateCommand
                (
                    command.User,
                    command.Payload,
                    commandProject.Id,
                    command.CommandId
                );

                var providerResults = await functionContext
                    .SendCommandAsync<ProviderProjectUserUpdateCommand>(providerCommand, commandProject)
                    .ConfigureAwait(true);

                var providerException = providerResults.Values?
                    .GetException();

                if (providerException != null)
                    throw providerException;
            }
            catch (Exception exc)
            {
                commandResult.Errors.Add(exc);

                throw;
            }
            finally
            {
                var commandException = commandResult.GetException();

                if (commandException is null)
                    functionContext.SetCustomStatus($"Command succeeded", log);
                else
                    functionContext.SetCustomStatus($"Command failed", log, commandException);

                commandResult.Result = command.Payload;

                functionContext.SetOutput(commandResult);
            }
        }
    }
}
