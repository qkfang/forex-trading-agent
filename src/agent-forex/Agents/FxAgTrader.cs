using Azure.AI.Projects;

namespace FxAgent.Agents;

public class FxAgTrader : BaseAgent
{
    public FxAgTrader(AIProjectClient aiProjectClient, string deploymentName)
        : base(aiProjectClient, "fxag-trader", deploymentName,
            "You are an FX trader assistant. Help traders interpret news feeds, evaluate open positions, and support trading decisions. Use available tools to access traders, news feeds, recommendations, and customer portfolios.")
    {
    }
}
