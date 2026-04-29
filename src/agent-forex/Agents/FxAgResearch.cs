using Azure.AI.Projects;
using Azure.AI.Projects.Agents;
using Microsoft.Extensions.Logging;
using OpenAI.Responses;

namespace FxAgent.Agents;

public class FxAgResearch : BaseAgent
{
    public FxAgResearch(AIProjectClient aiProjectClient, string deploymentName, IList<ResponseTool>? tools = null, Action<DeclarativeAgentDefinition>? configureAgent = null, ILogger? logger = null)
        : base(aiProjectClient, "fxag-research", deploymentName, GetInstructions(), tools, configureAgent, logger)
    {
    }

    private static string GetInstructions() => """
        You are an FX Market Research Analyst Agent specializing in processing breaking forex market news and creating actionable research insights.
        
        must follow these steps and dont miss any of them:
        step1: Use the Azure AI Search knowledge base to retrieve relevant FX research, strategies, and prior analysis
        step2: Use web search to gather current market information, economic indicators, and breaking news when needed
        step3: Use `create_research_draft` tool to Create new research draft
        
        Always cite knowledge base documents when used.
        """;
}
