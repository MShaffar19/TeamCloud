﻿/**
 *  Copyright (c) Microsoft Corporation.
 *  Licensed under the MIT License.
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using TeamCloud.Azure;
using TeamCloud.Model.Data;

namespace TeamCloud.Orchestrator.Orchestrations.Azure
{
    public class AzureSubscriptionPoolSelectActivity
    {
        private readonly IAzureSessionService azureSessionService;

        public AzureSubscriptionPoolSelectActivity(IAzureSessionService azureSessionService)
        {
            this.azureSessionService = azureSessionService ?? throw new ArgumentNullException(nameof(azureSessionService));
        }

        [FunctionName(nameof(AzureSubscriptionPoolSelectActivity))]
        public async Task<Guid> RunActivity(
            [ActivityTrigger] Project project)
        {
            if (project is null)
                throw new ArgumentNullException(nameof(project));

            var tasks = project.Type.Subscriptions
                .Select(subscription => GetResourceGroupCountAsync(subscription));

            var results = await Task.WhenAll(tasks)
                .ConfigureAwait(false);

            var subscriptions = results
                .Where(kvp => kvp.Key != Guid.Empty)
                .OrderBy(kvp => kvp.Value).ToArray();

            if (subscriptions.Length == 0)
                throw new ArgumentException("Subscription pool IDs are not valid or accessible.");
            else
                return subscriptions.First().Key;
        }

        private async Task<KeyValuePair<Guid, int>> GetResourceGroupCountAsync(Guid subscriptionId)
        {
            var azureSession = azureSessionService
                .CreateSession(subscriptionId);

            try
            {
                var resourceGroups = await azureSession.ResourceGroups
                    .ListAsync(true) // TODO: optimize this code using .ListByTagAsync instead and filter by some kind of TeamCloud tag
                    .ConfigureAwait(false);

                return new KeyValuePair<Guid, int>(subscriptionId, resourceGroups.Count());
            }
            catch
            {
                // TODO: add some logging and may some kind of logic to mark the given subscription id as broken

                return new KeyValuePair<Guid, int>(Guid.Empty, 0);
            }
        }
    }
}