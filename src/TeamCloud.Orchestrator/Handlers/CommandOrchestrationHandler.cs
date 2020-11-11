﻿/**
 *  Copyright (c) Microsoft Corporation.
 *  Licensed under the MIT License.
 */

using System;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using TeamCloud.Model.Commands;
using TeamCloud.Model.Commands.Core;
using TeamCloud.Orchestration;
using TeamCloud.Orchestrator.Orchestrations.Utilities;

namespace TeamCloud.Orchestrator.Handlers
{
    public sealed class CommandOrchestrationHandler : ICommandHandler
    {
        internal static string GetCommandOrchestrationName(ICommand command)
            => $"{command.GetType().Name}Orchestration";

        internal static string GetCommandOrchestrationInstanceId(Guid commandId)
            => commandId.ToString();

        internal static string GetCommandOrchestrationInstanceId(ICommand command)
            => GetCommandOrchestrationInstanceId(command.CommandId);

        internal static string GetCommandOrchestrationWrapperInstanceId(Guid commandId)
            => $"{GetCommandOrchestrationInstanceId(commandId)}-wrapper";

        internal static string GetCommandOrchestrationWrapperInstanceId(ICommand command)
            => GetCommandOrchestrationWrapperInstanceId(command.CommandId);

        public bool CanHandle(ICommand command)
        {
            if (command is null)
                throw new ArgumentNullException(nameof(command));

            var orchestrationName = GetCommandOrchestrationName(command);

            return FunctionsEnvironment.FunctionExists(orchestrationName);
        }

        public async Task<ICommandResult> HandleAsync(ICommand command, IDurableClient durableClient = null)
        {
            if (command is null)
                throw new ArgumentNullException(nameof(command));

            if (durableClient is null)
                throw new ArgumentNullException(nameof(durableClient));

            if (CanHandle(command))
            {
                var wrapperInstanceId = GetCommandOrchestrationWrapperInstanceId(command.CommandId);

                try
                {
                    _ = await durableClient
                        .StartNewAsync(nameof(OrchestratorCommandOrchestration), wrapperInstanceId, command)
                        .ConfigureAwait(false);
                }
                catch (InvalidOperationException exc)
                {
                    if ((await durableClient.GetCommandResultAsync(command).ConfigureAwait(false)) is null)
                    {
                        throw; // bubble exception - as we can't find a command wrapper orchestration.
                    }
                    else
                    {
                        throw new NotSupportedException($"Orchstration for command {command.CommandId} can only started once", exc);
                    }
                }

                return await durableClient
                    .GetCommandResultAsync(command)
                    .ConfigureAwait(false);
            }

            throw new NotImplementedException($"Missing orchestration to handle {command.GetType().Name} at {GetType()}");
        }
    }
}