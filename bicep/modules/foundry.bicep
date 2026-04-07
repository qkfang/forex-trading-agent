param name string
param location string
param tags object = {}

resource aiHub 'Microsoft.CognitiveServices/accounts@2025-10-01-preview' = {
  name: name
  location: 'WestUS3'
  tags: tags
  identity: {
    type: 'SystemAssigned'
  }
  sku: {
    name: 'S0'
  }
  kind: 'AIServices'
  properties: {
    allowProjectManagement: true
    customSubDomainName: name
    publicNetworkAccess: 'Enabled'
    disableLocalAuth: false
    networkAcls: {
      defaultAction: 'Allow'
    }
  }
}

resource aiProject 'Microsoft.CognitiveServices/accounts/projects@2025-06-01' = {
  parent: aiHub
  name: '${name}-project'
  location: 'WestUS3'
  identity: {
    type: 'SystemAssigned'
  }
  properties: {}
}

resource gpt4oDeployment 'Microsoft.CognitiveServices/accounts/deployments@2024-10-01' = {
  parent: aiHub
  name: 'gpt-5.4'
  sku: {
    name: 'GlobalStandard'
    capacity: 1000
  }
  properties: {
    model: {
      format: 'OpenAI'
      name: 'gpt-5.4'
      version: '2026-03-05'
    }
    versionUpgradeOption: 'OnceNewDefaultVersionAvailable'
    raiPolicyName: 'Microsoft.DefaultV2'
  }
}


resource fabricConnection 'Microsoft.CognitiveServices/accounts/connections@2025-10-01-preview' = {
  parent: aiHub
  name: 'fabric-data-agent'
  properties: {
    authType: 'CustomKeys'
    category: 'CustomKeys'
    target: '-'
    useWorkspaceManagedIdentity: false
    isSharedToAll: true
    credentials: {
      keys: {
      'workspace-id': '39ba570f-fadb-4b13-85b3-6938686a4a07'
      'artifact-id': '52e38886-b47c-48f5-9e14-157b0b9f1245'
      }
    }
    metadata: {
      type: 'fabric_dataagent_preview'
    }
  }
}

resource fabricProjectConnection 'Microsoft.CognitiveServices/accounts/projects/connections@2025-10-01-preview' = {
  parent: aiProject
  name: 'fabric-data-agent-project'
  properties: {
    authType: 'CustomKeys'
    category: 'CustomKeys'
    target: '-'
    useWorkspaceManagedIdentity: false
    isSharedToAll: false
    sharedUserList: []
    peRequirement: 'NotRequired'
    peStatus: 'NotApplicable'
    credentials: {
      keys: {
      'workspace-id': '39ba570f-fadb-4b13-85b3-6938686a4a07'
      'artifact-id': '52e38886-b47c-48f5-9e14-157b0b9f1245'
      }
    }
    metadata: {
      type: 'fabric_dataagent_preview'
    }
  }
}

output accountName string = aiHub.name
output resourceId string = aiHub.id
output endpoint string = aiHub.properties.endpoint
output deploymentName string = gpt4oDeployment.name
output projectName string = aiProject.name
output location string = location
output fabricConnectionName string = fabricConnection.name
