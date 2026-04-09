using Azure.AI.Projects;
using Azure.AI.Projects.Agents;
using Azure.Identity;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.Agents.AI;
using Microsoft.Agents.AI.Foundry;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.ApplicationInsights;
using ModelContextProtocol.Client;

// ── Config ──────────────────────────────────────────────────────────────────
var endpoint        = Environment.GetEnvironmentVariable("AZURE_AI_PROJECT_ENDPOINT")
                      ?? "https://fxag-foundry.services.ai.azure.com/api/projects/fxag-foundry-project";
var deploymentName  = Environment.GetEnvironmentVariable("AZURE_AI_MODEL_DEPLOYMENT_NAME")
                      ?? "gpt-5.4";
var apiIntgMcpUrl   = Environment.GetEnvironmentVariable("API_INTG_MCP_URL")
                      ?? "http://localhost:5005";
var appInsightsConnectionString =
    Environment.GetEnvironmentVariable("APPLICATIONINSIGHTS_CONNECTION_STRING")
    ?? "InstrumentationKey=91c3cb04-0d87-496a-ae8f-1c0294d30a4a;IngestionEndpoint=https://centralus-2.in.applicationinsights.azure.com/;LiveEndpoint=https://centralus.livediagnostics.monitor.azure.com/;ApplicationId=7a1a8b66-0689-4ff2-9914-4e2f9b5fbb0c";

// ── App Insights telemetry configuration ─────────────────────────────────────
var telemetryConfig = TelemetryConfiguration.CreateDefault();
telemetryConfig.ConnectionString = appInsightsConnectionString;
var telemetryClient = new TelemetryClient(telemetryConfig);

// ── Logger ───────────────────────────────────────────────────────────────────
using var loggerFactory = LoggerFactory.Create(b =>
    b.AddSimpleConsole(o => { o.SingleLine = true; o.TimestampFormat = "HH:mm:ss "; })
     .AddApplicationInsights(
         configureTelemetryConfiguration: c => c.ConnectionString = appInsightsConnectionString,
         configureApplicationInsightsLoggerOptions: _ => { })
     .SetMinimumLevel(LogLevel.Information));

var logger = loggerFactory.CreateLogger("TestAgent");

// ── Foundry client ───────────────────────────────────────────────────────────
logger.LogInformation("Connecting to Foundry project: {Endpoint}", endpoint);
var aiProjectClient = new AIProjectClient(new Uri(endpoint), new AzureCliCredential());

// ── MCP client → api-intg ────────────────────────────────────────────────────
logger.LogInformation("Connecting to api-intg MCP server: {Url}/mcp", apiIntgMcpUrl);
var mcpClient = await McpClient.CreateAsync(new HttpClientTransport(new HttpClientTransportOptions
{
    Endpoint = new Uri($"{apiIntgMcpUrl}/mcp"),
    Name     = "FxIntegrationApi"
}));

var mcpTools = await mcpClient.ListToolsAsync();
logger.LogInformation("MCP tools loaded ({Count}): {Tools}",
    mcpTools.Count, string.Join(", ", mcpTools.Select(t => t.Name)));

// ── Create Foundry agent v2 ──────────────────────────────────────────────────
const string agentId = "test-agent-v2";

var agentDefinition = new DeclarativeAgentDefinition(model: deploymentName)
{
    Instructions = """
        You are a test agent that helps verify connectivity to api-intg tools.
        Use the available tools to answer questions about customers and portfolios.
        Always confirm which tool you used and what data was returned.
        """
};

logger.LogInformation("Creating agent version for: {AgentId}", agentId);
var agentVersion = aiProjectClient.AgentAdministrationClient.CreateAgentVersion(
    agentId,
    new ProjectsAgentVersionCreationOptions(agentDefinition));

var agent = aiProjectClient.AsAIAgent(agentVersion, [.. mcpTools.Cast<AITool>()]);
logger.LogInformation("Agent {AgentId} ready with {ToolCount} tools", agentId, mcpTools.Count);

// ── Run test messages using OpenAI Responses API ─────────────────────────────
string[] testMessages =
[
    "List all customers."
];

foreach (var message in testMessages)
{
    logger.LogInformation("─── Sending: {Message}", message);
    try
    {
        var response = await agent.RunAsync(message);
        Console.WriteLine($"\n[Response]\n{response.Text}\n");
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Agent run failed for message: {Message}", message);
    }
}

// ── Cleanup ──────────────────────────────────────────────────────────────────
await mcpClient.DisposeAsync();
telemetryClient.Flush();
await Task.Delay(2000); // allow App Insights to flush buffered telemetry
logger.LogInformation("Done.");
