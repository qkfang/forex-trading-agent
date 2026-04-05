using Azure.AI.Projects;

namespace FxAgent.Agents;

public class FxAgSuggestion : BaseAgent
{
    public FxAgSuggestion(AIProjectClient aiProjectClient, string deploymentName)
        : base(aiProjectClient, "fxag-suggestion", deploymentName,
            "You are an FX trading suggestion engine. Based on market conditions, news, and portfolio data, provide actionable trading suggestions. Use available tools to access customer preferences, portfolios, trader recommendations, and research insights.")
    {
    }
}
