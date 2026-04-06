using Azure.AI.Extensions.OpenAI;
using Azure.AI.Projects;
using Azure.AI.Projects.Agents;
using Azure.Identity;
using OpenAI.Responses;

var projectEndpoint = Environment.GetEnvironmentVariable("FOUNDRY_PROJECT_ENDPOINT")
    ?? Environment.GetEnvironmentVariable("AZURE_AI_PROJECT_ENDPOINT")
    ?? "https://fxag-foundry.services.ai.azure.com/api/projects/fxag-foundry-project";

var modelDeploymentName = Environment.GetEnvironmentVariable("FOUNDRY_MODEL_NAME")
    ?? Environment.GetEnvironmentVariable("AZURE_AI_MODEL_DEPLOYMENT_NAME")
    ?? "gpt-5.4";

AIProjectClient projectClient = new(endpoint: new Uri(projectEndpoint), tokenProvider: new AzureCliCredential());

// Create agent with MCP tool pointing to Azure REST API specs via gitmcp.io
DeclarativeAgentDefinition agentDefinition = new(model: modelDeploymentName)
{
    Instructions = "You are a helpful agent that can use MCP tools to assist users. Use the available MCP tools to answer questions and perform tasks.",
    Tools = { ResponseTool.CreateMcpTool(
        serverLabel: "api-specs",
        serverUri: new Uri("https://gitmcp.io/Azure/azure-rest-api-specs"),
        toolCallApprovalPolicy: new McpToolCallApprovalPolicy(GlobalMcpToolCallApprovalPolicy.AlwaysRequireApproval)
    )}
};

Console.WriteLine("Creating agent version...");
ProjectsAgentVersion agentVersion = await projectClient.AgentAdministrationClient.CreateAgentVersionAsync(
    agentName: "sample-mcp19-agent",
    options: new(agentDefinition));

Console.WriteLine($"Agent version created: {agentVersion.Name}");

// Get responses client scoped to the agent
ProjectResponsesClient responseClient = projectClient.ProjectOpenAIClient.GetProjectResponsesClientForAgent(agentVersion.Name);

CreateResponseOptions nextResponseOptions = new()
{
    InputItems = { ResponseItem.CreateUserMessageItem("Please summarize the Azure REST API specifications Readme") }
};

ResponseResult? latestResponse = null;

while (nextResponseOptions is not null)
{
    latestResponse = await responseClient.CreateResponseAsync(nextResponseOptions);
    nextResponseOptions = null;

    foreach (ResponseItem responseItem in latestResponse.OutputItems)
    {
        if (responseItem is McpToolCallApprovalRequestItem mcpToolCall)
        {
            nextResponseOptions = new CreateResponseOptions()
            {
                PreviousResponseId = latestResponse.Id,
            };

            if (string.Equals(mcpToolCall.ServerLabel, "api-specs"))
            {
                Console.WriteLine($"Approving {mcpToolCall.ServerLabel}...");
                nextResponseOptions.InputItems.Add(
                    ResponseItem.CreateMcpApprovalResponseItem(approvalRequestId: mcpToolCall.Id, approved: true));
            }
            else
            {
                Console.WriteLine($"Rejecting unknown call {mcpToolCall.ServerLabel}...");
                nextResponseOptions.InputItems.Add(
                    ResponseItem.CreateMcpApprovalResponseItem(approvalRequestId: mcpToolCall.Id, approved: false));
            }
        }
    }
}

Console.WriteLine(latestResponse?.GetOutputText());

await projectClient.AgentAdministrationClient.DeleteAgentVersionAsync(
    agentName: agentVersion.Name,
    agentVersion: agentVersion.Version);

Console.WriteLine("Agent version deleted.");
