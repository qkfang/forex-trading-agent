using Azure.AI.Extensions.OpenAI;
using Azure.AI.Projects;
using Microsoft.Extensions.Logging;
using OpenAI.Responses;
using System.ClientModel;
using System.ClientModel.Primitives;
using System.Diagnostics;
using System.Text.Json;

namespace FxAgent.Agents;

public class FxAgIngestion
{
    private readonly AIProjectClient _aiProjectClient;
    private readonly ILogger _logger;
    private const string WorkflowName = "fxag-ingestion";

    public FxAgIngestion(AIProjectClient aiProjectClient, ILogger? logger = null)
    {
        _aiProjectClient = aiProjectClient;
        _logger = logger ?? LoggerFactory.Create(b => b.AddConsole()).CreateLogger<FxAgIngestion>();
        
        _logger.LogInformation("Initialized FxAgIngestion workflow client for agent: {WorkflowName}", WorkflowName);
    }

    public async Task<string> RunAsync(string message)
    {
        var sw = Stopwatch.StartNew();
        _logger.LogInformation("Starting workflow execution for: {Message}", message);
        
        // Create a conversation for this workflow execution
        var conversation = _aiProjectClient.ProjectOpenAIClient.GetProjectConversationsClient().CreateProjectConversation().Value;
        _logger.LogInformation("Created conversation: {ConversationId}", conversation.Id);
        
        // Use the agent-scoped responses client for correct authentication and base URL
        var responseClient = _aiProjectClient.ProjectOpenAIClient.GetProjectResponsesClientForAgent(WorkflowName);

        // Workflow agents use "conversation" (not the OpenAI standard "conversation_id"),
        // so we bypass SDK serialization and send the exact request body required
        var requestBody = BinaryData.FromObjectAsJson(new
        {
            model = WorkflowName,
            conversation = conversation.Id,
            input = new[] { new { type = "message", role = "user", content = message } }
        });

        ClientResult rawResult = await responseClient.CreateResponseAsync(
            BinaryContent.Create(requestBody));

        var responseResult = ModelReaderWriter.Read<ResponseResult>(
            rawResult.GetRawResponse().Content,
            ModelReaderWriterOptions.Json);

        sw.Stop();
        _logger.LogInformation("Workflow {WorkflowName} completed in {Duration}ms", WorkflowName, sw.ElapsedMilliseconds);

        return responseResult?.GetOutputText() ?? string.Empty;
    }
}
