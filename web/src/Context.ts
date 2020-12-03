// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import React from 'react';
import { Component, ComponentTemplate, Organization, Project, User, UserDefinition } from 'teamcloud'
import { GraphUser, ProjectMember } from './model';

export const GraphUserContext = React.createContext({
    graphUser: undefined as GraphUser | undefined,
    setGraphUser: (graphUser?: GraphUser) => { }
});

export const OrgContext = React.createContext({
    org: undefined as Organization | undefined,
    orgs: undefined as Organization[] | undefined,
    onOrgSelected: (org?: Organization) => { },
    user: undefined as User | undefined,
    // setUser: (user?: User) => { },
    project: undefined as Project | undefined,
    projects: undefined as Project[] | undefined,
    onProjectSelected: (project?: Project) => { }
    // setMembers: (members?: Member[]) => { },
});

export const ProjectContext = React.createContext({
    org: undefined as Organization | undefined,
    user: undefined as User | undefined,
    project: undefined as Project | undefined,
    members: undefined as ProjectMember[] | undefined,
    components: undefined as Component[] | undefined,
    templates: undefined as ComponentTemplate[] | undefined,
    onAddUsers: (users: UserDefinition[]) => Promise.resolve(),
});

