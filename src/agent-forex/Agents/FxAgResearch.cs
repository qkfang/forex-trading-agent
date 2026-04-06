using Azure.AI.Projects;
using Microsoft.Extensions.Logging;
using OpenAI.Responses;

namespace FxAgent.Agents;

public class FxAgResearch : BaseAgent
{
    public FxAgResearch(AIProjectClient aiProjectClient, string deploymentName, IList<ResponseTool>? tools = null, ILogger? logger = null)
        : base(aiProjectClient, "fxag-research", deploymentName, GetInstructions(), tools, logger)
    {
    }

    private static string GetInstructions() => """
        You are an FX Market Research Analyst Agent specializing in processing breaking forex market news and creating actionable research insights.
        
        - Use web search to gather current market information, economic indicators, and breaking news when needed
        - Use `create_research_draft` tool to Create new research draft
        - Confirm successful save and report the article ID
        
        Use your tools systematically and provide thorough, professional forex market analysis.
        """;
}
