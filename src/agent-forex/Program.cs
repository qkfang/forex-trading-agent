using Azure.AI.Extensions.OpenAI;
using Azure.AI.Projects;
using Azure.Identity;
using FxAgent.Agents;
using OpenAI.Responses;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApplicationInsightsTelemetry();

builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();
builder.Logging.AddApplicationInsights();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHealthChecks();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapHealthChecks("/health");

app.MapGet("/", () => Results.Redirect("/swagger"));

var logger = app.Services.GetRequiredService<ILogger<Program>>();

var endpoint = app.Configuration["AZURE_AI_PROJECT_ENDPOINT"]
    ?? throw new InvalidOperationException("AZURE_AI_PROJECT_ENDPOINT is not set.");
var deploymentName = app.Configuration["AZURE_AI_MODEL_DEPLOYMENT_NAME"]
    ?? throw new InvalidOperationException("AZURE_AI_MODEL_DEPLOYMENT_NAME is not set.");
var bingConnectionName = app.Configuration["BING_CONNECTION_NAME"];

AIProjectClient aiProjectClient = new(new Uri(endpoint), new AzureCliCredential());

var apiMcpUrl = app.Configuration["API_INTG_MCP_URL"];
var tradingMcpUrl = app.Configuration["TRADING_PLATFORM_MCP_URL"];

var apiIntgTool = ResponseTool.CreateMcpTool(
    serverLabel: "api-intg",
    serverUri: new Uri($"{apiMcpUrl}/mcp"),
    toolCallApprovalPolicy: new McpToolCallApprovalPolicy(GlobalMcpToolCallApprovalPolicy.AlwaysRequireApproval)
);

var tradingTool = ResponseTool.CreateMcpTool(
    serverLabel: "trading-platform",
    serverUri: new Uri($"{tradingMcpUrl}/mcp"),
    toolCallApprovalPolicy: new McpToolCallApprovalPolicy(GlobalMcpToolCallApprovalPolicy.AlwaysRequireApproval)
);

var loggerFactory = app.Services.GetRequiredService<ILoggerFactory>();

ResponseTool? bingTool = null;
if (!string.IsNullOrEmpty(bingConnectionName))
{
    try
    {
        var bingConnection = aiProjectClient.Connections.GetConnection(connectionName: bingConnectionName);
        bingTool = ResponseTool.CreateBingGroundingTool(new BingGroundingSearchToolOptions(
            searchConfigurations: [new BingGroundingSearchConfiguration(projectConnectionId: bingConnection.Id)]
        ));
        logger.LogInformation("Bing grounding tool enabled for research agent");
    }
    catch (Exception ex)
    {
        logger.LogWarning(ex, "Failed to configure Bing grounding tool");
    }
}

var researchTools = new List<ResponseTool> { apiIntgTool };
if (bingTool != null)
    researchTools.Add(bingTool);

var researchAgent = new FxAgResearch(aiProjectClient, deploymentName, researchTools, loggerFactory.CreateLogger<FxAgResearch>());
var suggestionAgent = new FxAgSuggestion(aiProjectClient, deploymentName, [apiIntgTool], loggerFactory.CreateLogger<FxAgSuggestion>());
var insightAgent = new FxAgInsight(aiProjectClient, deploymentName, [apiIntgTool], loggerFactory.CreateLogger<FxAgInsight>());
var traderAgent = new FxAgTrader(aiProjectClient, deploymentName, [tradingTool], loggerFactory.CreateLogger<FxAgTrader>());

app.MapPost("/research", async (ChatRequest request) =>
{
    logger.LogInformation("Research request: {Message}", request.Message);
    var response = await researchAgent.RunAsync(request.Message);
    return Results.Ok(new { response });
});

app.MapPost("/suggestion", async (ChatRequest request) =>
{
    logger.LogInformation("Suggestion request: {Message}", request.Message);
    var response = await suggestionAgent.RunAsync(request.Message);
    return Results.Ok(new { response });
});

app.MapPost("/trader", async (ChatRequest request) =>
{
    logger.LogInformation("Trader request: {Message}", request.Message);
    var response = await traderAgent.RunAsync(request.Message);
    return Results.Ok(new { response });
});

app.MapPost("/insight", async (ChatRequest request) =>
{
    logger.LogInformation("Insight request: {Message}", request.Message);
    var response = await insightAgent.RunAsync(request.Message);
    return Results.Ok(new { response });
});

await app.RunAsync();

record ChatRequest(string Message);
