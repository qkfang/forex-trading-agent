using Azure.AI.Extensions.OpenAI;
using Azure.AI.Projects;
using Azure.Identity;
using OpenAI.Responses;
using System.Text.Json;

var projectEndpoint = Environment.GetEnvironmentVariable("AZURE_AI_PROJECT_ENDPOINT")
    ?? "https://fxag-foundry.services.ai.azure.com/api/projects/fxag-foundry-project";

var projectClient = new AIProjectClient(new Uri(projectEndpoint), new AzureCliCredential());

var newsArticle = new
{
    id = 4,
    title = "US Expands Tariffs on Chinese Goods, Renewing Trade War Fears",
    summary = "Washington's new 25% tariffs on Chinese tech exports send USD higher and EM currencies lower.",
    content = "The United States announced sweeping new 25% tariffs on a broad range of Chinese-manufactured technology products, including electric vehicles, solar panels and advanced semiconductors. The move, framed as a national security measure, drew an immediate pledge of retaliation from Beijing. The US dollar index (DXY) jumped 0.7% as investors priced in a more restrictive global trade environment, while the offshore yuan weakened past 7.35 against the dollar — a level not seen since the 2022 peak. Emerging market currencies with close trade ties to China, including the Thai baht, South Korean won and Malaysian ringgit, fell sharply. Equity markets in Europe and Asia sold off, with tech-heavy indices leading losses on fears of supply-chain disruption. Strategists warn that a full-blown trade war could shave 0.5–0.8 percentage points from global GDP growth in 2026.",
    type = "Bad",
    category = "Macro",
    publishedDate = "2026-03-16T10:15:00",
    author = "FX News Team",
    isPublished = true,
    publishedAt = "2026-03-16T10:20:00",
    source = "FX News Centre"
};

var analysisRequest = $"""
    who is the customer to suggest?

    {JsonSerializer.Serialize(newsArticle, new JsonSerializerOptions { WriteIndented = true })}
    """;

// Get responses client for the existing suggestion agent
var responseClient = projectClient.ProjectOpenAIClient.GetProjectResponsesClientForAgent("fxag-suggestion");

var nextOptions = new CreateResponseOptions
{
    InputItems = { ResponseItem.CreateUserMessageItem(analysisRequest) }
};

ResponseResult? result = null;

while (nextOptions is not null)
{
    result = await responseClient.CreateResponseAsync(nextOptions);
    nextOptions = null;

    foreach (var item in result.OutputItems)
    {
        if (item is McpToolCallApprovalRequestItem mcpCall)
        {
            Console.WriteLine($"Auto-approving MCP tool call: {mcpCall.ServerLabel}");
            nextOptions ??= new CreateResponseOptions { PreviousResponseId = result.Id };
            nextOptions.InputItems.Add(
                ResponseItem.CreateMcpApprovalResponseItem(mcpCall.Id, approved: true));
        }
    }
}

Console.WriteLine(result?.GetOutputText());
