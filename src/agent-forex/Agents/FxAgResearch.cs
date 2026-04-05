using Azure.AI.Projects;

namespace FxAgent.Agents;

public class FxAgResearch : BaseAgent
{
    public FxAgResearch(AIProjectClient aiProjectClient, string deploymentName, string? bingConnectionName = null)
        : base(aiProjectClient, "fxag-research", deploymentName,
            "You are an FX market research analyst. Analyze currency research articles, identify patterns, and summarize research findings. Use Bing search to gather current market information, economic data, and breaking news. Use available tools to access research data, articles, patterns, and drafts.",
            bingConnectionName)
    {
    }
}
