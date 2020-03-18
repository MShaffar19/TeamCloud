# coding=utf-8
# --------------------------------------------------------------------------
# Copyright (c) Microsoft Corporation. All rights reserved.
# Licensed under the MIT License. See License.txt in the project root for
# license information.
#
# Code generated by Microsoft (R) AutoRest Code Generator.
# Changes may cause incorrect behavior and will be lost if the code is
# regenerated.
# --------------------------------------------------------------------------

from .project import Project


class ProjectDataResultData(Project):
    """ProjectDataResultData.

    Variables are only populated by the server, and will be ignored when
    sending a request.

    :ivar partition_key:
    :vartype partition_key: str
    :param id:
    :type id: str
    :param name:
    :type name: str
    :param type:
    :type type: ~teamcloud.models.ProjectTypeModel
    :param identity:
    :type identity: ~teamcloud.models.ProjectIdentity
    :param resource_group:
    :type resource_group: ~teamcloud.models.ProjectResourceGroup
    :param team_cloud_id:
    :type team_cloud_id: str
    :param users:
    :type users: list[~teamcloud.models.User]
    :param tags:
    :type tags: dict[str, str]
    :param properties:
    :type properties: dict[str, str]
    """

    _validation = {
        'partition_key': {'readonly': True},
    }

    _attribute_map = {
        'partition_key': {'key': 'partitionKey', 'type': 'str'},
        'id': {'key': 'id', 'type': 'str'},
        'name': {'key': 'name', 'type': 'str'},
        'type': {'key': 'type', 'type': 'ProjectTypeModel'},
        'identity': {'key': 'identity', 'type': 'ProjectIdentity'},
        'resource_group': {'key': 'resourceGroup', 'type': 'ProjectResourceGroup'},
        'team_cloud_id': {'key': 'teamCloudId', 'type': 'str'},
        'users': {'key': 'users', 'type': '[User]'},
        'tags': {'key': 'tags', 'type': '{str}'},
        'properties': {'key': 'properties', 'type': '{str}'},
    }

    def __init__(self, **kwargs):
        super(ProjectDataResultData, self).__init__(**kwargs)
