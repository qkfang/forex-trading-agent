using Azure.AI.Projects;
using Microsoft.Extensions.Logging;
using OpenAI.Responses;

namespace FxAgent.Agents;

public class FxAgInsight : BaseAgent
{
    public FxAgInsight(AIProjectClient aiProjectClient, string deploymentName, IList<ResponseTool>? tools = null, ILogger? logger = null)
        : base(aiProjectClient, "fxag-insight", deploymentName,
            "You are an FX market insight specialist. Deliver market insights, portfolio performance summaries, and trend analysis. Use available tools to access customer data, portfolios, trading history, research articles, and patterns.",
            tools, logger)
    {
    }
}
