
az group create --name 'rg-fx-agent' --location 'centralus'

az deployment group create --name 'fx-agent-dev' --resource-group 'rg-fx-agent' --template-file './main.bicep' --parameters './parameters.dev.json'


