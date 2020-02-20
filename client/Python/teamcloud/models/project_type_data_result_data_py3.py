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

from .project_type_py3 import ProjectType


class ProjectTypeDataResultData(ProjectType):
    """ProjectTypeDataResultData.

    Variables are only populated by the server, and will be ignored when
    sending a request.

    :ivar partition_key:
    :vartype partition_key: str
    :param id:
    :type id: str
    :param default:
    :type default: bool
    :param region:
    :type region: str
    :param subscriptions:
    :type subscriptions: list[str]
    :param subscription_capacity:
    :type subscription_capacity: int
    :param resource_group_name_prefix:
    :type resource_group_name_prefix: str
    :param providers:
    :type providers: list[~teamcloud.models.ProjectTypeProvider]
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
        'default': {'key': 'default', 'type': 'bool'},
        'region': {'key': 'region', 'type': 'str'},
        'subscriptions': {'key': 'subscriptions', 'type': '[str]'},
        'subscription_capacity': {'key': 'subscriptionCapacity', 'type': 'int'},
        'resource_group_name_prefix': {'key': 'resourceGroupNamePrefix', 'type': 'str'},
        'providers': {'key': 'providers', 'type': '[ProjectTypeProvider]'},
        'tags': {'key': 'tags', 'type': '{str}'},
        'properties': {'key': 'properties', 'type': '{str}'},
    }

    def __init__(self, *, id: str=None, default: bool=None, region: str=None, subscriptions=None, subscription_capacity: int=None, resource_group_name_prefix: str=None, providers=None, tags=None, properties=None, **kwargs) -> None:
        super(ProjectTypeDataResultData, self).__init__(id=id, default=default, region=region, subscriptions=subscriptions, subscription_capacity=subscription_capacity, resource_group_name_prefix=resource_group_name_prefix, providers=providers, tags=tags, properties=properties, **kwargs)
