using Azure.AI.Projects;
using Azure.AI.Agents.Persistent;
using Microsoft.Agents.AI.Foundry;
using OpenAI.Responses;
using System.Text.Json;

namespace FxAgent.Agents;

public abstract class BaseAgent
{
    protected readonly FoundryAgent _agent;
    protected readonly AIProjectClient _aiProjectClient;

    protected BaseAgent(AIProjectClient aiProjectClient, string agentId, string deploymentName, string instructions)
    {
        _aiProjectClient = aiProjectClient;
        
        var tools = McpToolDefinitions.GetAllToolDefinitions()
            .Select(t => new ResponseTool
            {
                Type = "function",
                Function = new ResponseFunction
                {
                    Name = t.FunctionName,
                    Description = t.FunctionName,
                    Parameters = t.Parameters
                }
            })
            .ToList();
        
        var agentDefinition = new DeclarativeAgentDefinition(model: deploymentName)
        {
            Instructions = instructions
        };
        
        foreach (var tool in tools)
        {
            agentDefinition.Tools.Add(tool);
        }
        
        var agentVersion = aiProjectClient.AgentAdministrationClient.CreateAgentVersion(
            agentId,
            new ProjectsAgentVersionCreationOptions(agentDefinition));

        _agent = aiProjectClient.AsAIAgent(agentVersion);
    }

    public async Task<string> RunAsync(string message)
    {
        var response = await _agent.InvokeAsync(message);
        
        if (response?.Messages?.LastOrDefault()?.Content is string content)
        {
            return content;
        }

        return "Agent run failed or produced no output.";
    }
}
