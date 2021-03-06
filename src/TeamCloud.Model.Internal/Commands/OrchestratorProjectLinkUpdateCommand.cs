﻿/**
 *  Copyright (c) Microsoft Corporation.
 *  Licensed under the MIT License.
 */

using TeamCloud.Model.Data;

namespace TeamCloud.Model.Commands
{
    public sealed class OrchestratorProjectLinkUpdateCommand : OrchestratorUpdateCommand<ProjectLinkDocument, OrchestratorProjectLinkUpdateCommandResult>
    {
        public OrchestratorProjectLinkUpdateCommand(UserDocument user, ProjectLinkDocument payload, string projectId) : base(user, payload)
            => ProjectId = projectId;
    }
}
