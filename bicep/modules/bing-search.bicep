@description('Name of Bing Search resource')
param name string

@description('Azure region for the resource')
param location string = resourceGroup().location

@description('Tags to apply to the resource')
param tags object = {}

@description('SKU for Bing Search - S1 is standard tier')
param sku string = 'S1'

resource bingSearch 'Microsoft.Bing/accounts@2020-06-10' = {
  name: name
  location: 'global'
  kind: 'Bing.Search.v7'
  sku: {
    name: sku
  }
  tags: tags
  properties: {
    statisticsEnabled: false
  }
}

output id string = bingSearch.id
output name string = bingSearch.name
output endpoint string = 'https://api.bing.microsoft.com/v7.0/search'
output key string = bingSearch.listKeys().key1
