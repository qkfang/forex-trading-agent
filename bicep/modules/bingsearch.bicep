param name string
param location string
param tags object = {}
param sku string = 'S1'
param webAppPrincipalId string = ''
param principals array = []

resource bingSearch 'Microsoft.Bing/accounts@2020-06-10' = {
  name: name
  location: 'global'
  tags: tags
  sku: {
    name: sku
  }
  kind: 'Bing.Search.v7'
  properties: {
    statisticsEnabled: false
  }
}

var searchServiceContributorRoleId = '7ca78c08-252a-4471-8644-bb5ff32d4ba0'

resource webAppRoleAssignment 'Microsoft.Authorization/roleAssignments@2022-04-01' = if (!empty(webAppPrincipalId)) {
  name: guid(bingSearch.id, webAppPrincipalId, searchServiceContributorRoleId)
  scope: bingSearch
  properties: {
    roleDefinitionId: subscriptionResourceId('Microsoft.Authorization/roleDefinitions', searchServiceContributorRoleId)
    principalId: webAppPrincipalId
    principalType: 'ServicePrincipal'
  }
}

resource userRoleAssignment 'Microsoft.Authorization/roleAssignments@2022-04-01' = [for principal in principals: {
  name: guid(bingSearch.id, principal.id, searchServiceContributorRoleId)
  scope: bingSearch
  properties: {
    roleDefinitionId: subscriptionResourceId('Microsoft.Authorization/roleDefinitions', searchServiceContributorRoleId)
    principalId: principal.id
    principalType: principal.principalType
  }
}]

output id string = bingSearch.id
output name string = bingSearch.name
output endpoint string = bingSearch.properties.endpoint
