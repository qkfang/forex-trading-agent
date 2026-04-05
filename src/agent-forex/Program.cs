using System.ComponentModel;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.ClientModel.Primitives;
using Azure.AI.AgentServer.AgentFramework.Extensions;
using Azure.AI.OpenAI;
using Azure.AI.Projects;
using Azure.AI.Projects.OpenAI;
using Azure.Identity;
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;

var endpoint = Environment.GetEnvironmentVariable("AZURE_AI_PROJECT_ENDPOINT")
    ?? throw new InvalidOperationException("AZURE_AI_PROJECT_ENDPOINT is not set.");
var deploymentName = Environment.GetEnvironmentVariable("MODEL_DEPLOYMENT_NAME") ?? "gpt-4.1";
var crmBrokerUrl = Environment.GetEnvironmentVariable("CRM_BROKER_URL") ?? "http://localhost:5148";

Console.WriteLine($"Project Endpoint: {endpoint}");
Console.WriteLine($"Model Deployment: {deploymentName}");
Console.WriteLine($"CRM Broker URL: {crmBrokerUrl}");

var httpClient = new HttpClient { BaseAddress = new Uri(crmBrokerUrl) };
var jsonOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

// ── Tool functions that call the crm-broker API ──────────────────────────

[Description("Get the current AUD/USD exchange rate quote with bid, ask, mid, and spread.")]
string GetFxQuote()
{
    try
    {
        var response = httpClient.GetAsync("/api/fx/quote").Result;
        response.EnsureSuccessStatusCode();
        return response.Content.ReadAsStringAsync().Result;
    }
    catch (Exception ex)
    {
        return $"Error fetching quote: {ex.Message}";
    }
}

[Description("Get the current market status including trend direction, volatility, and day statistics for AUD/USD.")]
string GetMarketStatus()
{
    try
    {
        var response = httpClient.GetAsync("/api/fx/status").Result;
        response.EnsureSuccessStatusCode();
        return response.Content.ReadAsStringAsync().Result;
    }
    catch (Exception ex)
    {
        return $"Error fetching market status: {ex.Message}";
    }
}

[Description("Get recent OHLC price history candles for AUD/USD.")]
string GetPriceHistory(
    [Description("Number of candle bars to retrieve (1-500, default 20)")] int bars = 20)
{
    try
    {
        bars = Math.Clamp(bars, 1, 500);
        var response = httpClient.GetAsync($"/api/fx/history?bars={bars}").Result;
        response.EnsureSuccessStatusCode();
        return response.Content.ReadAsStringAsync().Result;
    }
    catch (Exception ex)
    {
        return $"Error fetching price history: {ex.Message}";
    }
}

[Description("Get all trading accounts with their summary information.")]
string GetAccounts()
{
    try
    {
        var response = httpClient.GetAsync("/api/accounts").Result;
        response.EnsureSuccessStatusCode();
        return response.Content.ReadAsStringAsync().Result;
    }
    catch (Exception ex)
    {
        return $"Error fetching accounts: {ex.Message}";
    }
}

[Description("Get the balance sheet for a specific trading account, including open positions and recent transactions.")]
string GetAccountBalance(
    [Description("The account ID (e.g., 1, 2, or 3)")] int accountId)
{
    try
    {
        var response = httpClient.GetAsync($"/api/accounts/{accountId}/balance").Result;
        response.EnsureSuccessStatusCode();
        return response.Content.ReadAsStringAsync().Result;
    }
    catch (Exception ex)
    {
        return $"Error fetching balance for account {accountId}: {ex.Message}";
    }
}

[Description("Execute a buy trade on AUD/USD for a specific account.")]
string ExecuteBuy(
    [Description("The account ID")] int accountId,
    [Description("Number of lots to buy (e.g., 0.1, 0.5, 1.0)")] decimal lots)
{
    try
    {
        var content = JsonContent.Create(new { currencyPair = "AUD/USD", lots });
        var response = httpClient.PostAsync($"/api/accounts/{accountId}/buy", content).Result;
        return response.Content.ReadAsStringAsync().Result;
    }
    catch (Exception ex)
    {
        return $"Error executing buy: {ex.Message}";
    }
}

[Description("Execute a sell trade on AUD/USD for a specific account.")]
string ExecuteSell(
    [Description("The account ID")] int accountId,
    [Description("Number of lots to sell (e.g., 0.1, 0.5, 1.0)")] decimal lots)
{
    try
    {
        var content = JsonContent.Create(new { currencyPair = "AUD/USD", lots });
        var response = httpClient.PostAsync($"/api/accounts/{accountId}/sell", content).Result;
        return response.Content.ReadAsStringAsync().Result;
    }
    catch (Exception ex)
    {
        return $"Error executing sell: {ex.Message}";
    }
}

[Description("Close an open position for an account.")]
string ClosePosition(
    [Description("The account ID")] int accountId,
    [Description("The position ID to close (e.g., POS001)")] string positionId)
{
    try
    {
        var response = httpClient.PostAsync($"/api/accounts/{accountId}/close/{positionId}", null).Result;
        return response.Content.ReadAsStringAsync().Result;
    }
    catch (Exception ex)
    {
        return $"Error closing position: {ex.Message}";
    }
}

[Description("Get recent FX transaction records.")]
string GetTransactions(
    [Description("Maximum number of transactions to return (default 20)")] int limit = 20)
{
    try
    {
        var response = httpClient.GetAsync($"/api/fx/transactions?limit={limit}").Result;
        response.EnsureSuccessStatusCode();
        return response.Content.ReadAsStringAsync().Result;
    }
    catch (Exception ex)
    {
        return $"Error fetching transactions: {ex.Message}";
    }
}

// ── Build the Agent ──────────────────────────────────────────────────────

var credential = new DefaultAzureCredential();
AIProjectClient projectClient = new AIProjectClient(new Uri(endpoint), credential);

// ── Provision Foundry Agents (create if not exist) ───────────────────────

await ProvisionFoundryAgentsAsync(projectClient, deploymentName);

ClientConnection connection = projectClient.GetConnection(typeof(AzureOpenAIClient).FullName!);

if (!connection.TryGetLocatorAsUri(out Uri? openAiEndpoint) || openAiEndpoint is null)
{
    throw new InvalidOperationException("Failed to get OpenAI endpoint from project connection.");
}
openAiEndpoint = new Uri($"https://{openAiEndpoint.Host}");
Console.WriteLine($"OpenAI Endpoint: {openAiEndpoint}");

var chatClient = new AzureOpenAIClient(openAiEndpoint, credential)
    .GetChatClient(deploymentName)
    .AsIChatClient()
    .AsBuilder()
    .UseOpenTelemetry(sourceName: "Agents", configure: cfg => cfg.EnableSensitiveData = false)
    .Build();

var agent = new ChatClientAgent(chatClient,
    name: "ForexTradingAgent",
    instructions: """
        You are an expert forex trading assistant for AUD/USD currency pair.

        Your capabilities:
        - View real-time FX quotes (bid/ask/spread)
        - Check market status (trend, volatility, day statistics)
        - View price history candles
        - List trading accounts and their balances
        - Execute buy and sell trades
        - Close open positions
        - View transaction history

        Guidelines:
        - Always show the current quote before executing trades
        - Warn about risks before executing large trades
        - Present account information clearly with key metrics
        - When asked about market conditions, use both quote and status tools
        - Format currency values to 4 decimal places for rates and 2 for account balances
        """,
    tools:
    [
        AIFunctionFactory.Create(GetFxQuote),
        AIFunctionFactory.Create(GetMarketStatus),
        AIFunctionFactory.Create(GetPriceHistory),
        AIFunctionFactory.Create(GetAccounts),
        AIFunctionFactory.Create(GetAccountBalance),
        AIFunctionFactory.Create(ExecuteBuy),
        AIFunctionFactory.Create(ExecuteSell),
        AIFunctionFactory.Create(ClosePosition),
        AIFunctionFactory.Create(GetTransactions)
    ])
    .AsBuilder()
    .UseOpenTelemetry(sourceName: "Agents", configure: cfg => cfg.EnableSensitiveData = false)
    .Build();

Console.WriteLine("Forex Trading Agent Server running on http://localhost:8088");
await agent.RunAIAgentAsync(telemetrySourceName: "Agents");

// ── Foundry Agent Provisioning ───────────────────────────────────────────

static async Task ProvisionFoundryAgentsAsync(AIProjectClient projectClient, string model)
{
    var agentConfigs = new Dictionary<string, string>
    {
        ["fx-agent-research"] = """
            You are a forex market research analyst specializing in AUD/USD.

            Your role:
            - Analyze price history and identify technical patterns (support/resistance, trends, reversals)
            - Summarize current market conditions using real-time quotes and market status
            - Provide data-driven market commentary and outlook
            - Track volatility, spread changes, and day statistics

            Guidelines:
            - Base all analysis on actual market data
            - Clearly distinguish between observed data and analytical interpretation
            - Present findings in a structured, professional format
            - Highlight key risk factors and unusual market conditions
            """,

        ["fx-agent-suggestion"] = """
            You are a forex trading suggestion advisor for AUD/USD.

            Your role:
            - Review customer portfolios and open positions
            - Provide personalized trade suggestions based on account balances and risk exposure
            - Recommend entry/exit points using current quotes and market status
            - Assess risk/reward ratios for proposed trades

            Guidelines:
            - Always consider the customer's existing positions before suggesting trades
            - Provide clear rationale for each suggestion
            - Include risk warnings with every recommendation
            - Never guarantee profits or make misleading claims
            """,

        ["fx-agent-trader"] = """
            You are a forex trade execution agent for AUD/USD.

            Your role:
            - Execute buy and sell orders on trading accounts
            - Monitor open positions and account balances
            - Close positions when requested
            - Report trade confirmations and updated account status

            Guidelines:
            - Always verify the current quote before executing a trade
            - Confirm order details (account, direction, lot size) before execution
            - Warn about risks for large position sizes relative to account balance
            - Report execution results clearly with fill price and updated balance
            """,

        ["fx-agent-chatbot"] = """
            You are a general-purpose forex trading assistant for AUD/USD.

            Your capabilities:
            - Answer questions about current market conditions (quotes, trends, volatility)
            - Explain trading concepts and terminology
            - Help users navigate their accounts and positions
            - Provide price history and transaction records
            - Assist with trade execution when requested

            Guidelines:
            - Be conversational and helpful
            - Provide clear, concise answers
            - When discussing trades, always show relevant market data first
            - Format currency values to 4 decimal places for rates and 2 for account balances
            """
    };

    Console.WriteLine("Provisioning Foundry agents...");

    var existingNames = new HashSet<string>();
    await foreach (var existing in projectClient.Agents.GetAgentsAsync())
    {
        existingNames.Add(existing.Name);
    }

    foreach (var (agentName, instructions) in agentConfigs)
    {
        if (existingNames.Contains(agentName))
        {
            Console.WriteLine($"  Agent '{agentName}' already exists, skipping.");
            continue;
        }

        var definition = new PromptAgentDefinition(model: model)
        {
            Instructions = instructions
        };

        var agentVersion = await projectClient.Agents.CreateAgentVersionAsync(
            agentName: agentName,
            options: new AgentVersionCreationOptions(definition));

        Console.WriteLine($"  Created agent '{agentName}' (version: {agentVersion.Value.Version})");
    }

    Console.WriteLine("Foundry agent provisioning complete.");
}
