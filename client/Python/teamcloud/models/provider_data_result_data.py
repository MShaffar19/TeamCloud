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

from .provider import Provider


class ProviderDataResultData(Provider):
    """ProviderDataResultData.

    :param id:
    :type id: str
    :param url:
    :type url: str
    :param auth_code:
    :type auth_code: str
    :param principal_id:
    :type principal_id: str
    :param events:
    :type events: list[str]
    :param properties:
    :type properties: dict[str, str]
    :param registered:
    :type registered: datetime
    """

    _attribute_map = {
        'id': {'key': 'id', 'type': 'str'},
        'url': {'key': 'url', 'type': 'str'},
        'auth_code': {'key': 'authCode', 'type': 'str'},
        'principal_id': {'key': 'principalId', 'type': 'str'},
        'events': {'key': 'events', 'type': '[str]'},
        'properties': {'key': 'properties', 'type': '{str}'},
        'registered': {'key': 'registered', 'type': 'iso-8601'},
    }

    def __init__(self, **kwargs):
        super(ProviderDataResultData, self).__init__(**kwargs)
