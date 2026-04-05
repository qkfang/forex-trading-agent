# MCP Tools for FX Agent

## Overview

This implementation provides MCP (Model Context Protocol) tool endpoints for all RESTful APIs in the integration service. All agents (Research, Suggestion, Trader, Insight) have access to these tools.

## Available MCP Tools

### Customer Management
- `get_all_customers` - Get all customers with their portfolios
- `get_customer` - Get customer by ID
- `create_customer` - Create new customer (name, email, phone, company)
- `update_customer` - Update customer information
- `delete_customer` - Delete customer

### Portfolio Management
- `get_customer_portfolios` - Get all portfolios for a specific customer
- `get_portfolio` - Get portfolio by ID
- `create_portfolio` - Create new portfolio position (currencyPair, direction, amount, entryRate)
- `update_portfolio` - Update portfolio position
- `delete_portfolio` - Delete portfolio position

### Trader Management
- `get_all_traders` - Get all traders with recommendations and feeds
- `get_trader` - Get trader by ID
- `create_trader` - Create new trader (name, email, expertise, yearsActive)

### Research Management
- `get_all_research_articles` - Get all research articles
- `get_research_article` - Get research article by ID
- `create_research_article` - Create new research article (title, content, currencyPair, analysis)

- `get_all_research_drafts` - Get all research drafts
- `create_research_draft` - Create new research draft (title, content, status)

- `get_all_research_patterns` - Get all identified trading patterns
- `create_research_pattern` - Create new pattern observation (patternName, description, currencyPair)

### Customer Preferences & History
- `get_customer_preferences` - Get customer trading preferences by customer ID
- `update_customer_preferences` - Update customer preferences (riskLevel, preferredPairs, tradingObjective)
- `get_customer_history` - Get customer trading history by customer ID

### Trader Feeds & Recommendations
- `get_trader_news` - Get news feeds for a specific trader
- `get_trader_recommendations` - Get recommendations from a specific trader

## Architecture

### Components

1. **McpTools.cs** - Static methods that call the REST API endpoints via HttpClient
2. **McpToolDefinitions.cs** - Tool definitions and invocation routing
3. **BaseAgent.cs** - Base class for all agents with MCP tool support and execution loop
4. **Agent Classes** - FxAgResearch, FxAgSuggestion, FxAgTrader, FxAgInsight

### Tool Invocation Flow

1. User sends message to agent endpoint
2. Agent creates thread and runs conversation
3. Agent detects function calls in response
4. `McpToolDefinitions.InvokeToolAsync()` routes to appropriate tool
5. `McpTools` class executes HTTP call to integration API
6. Result returned to agent for response generation

## Configuration

Set `API_BASE_URL` environment variable to point to the integration API:

```
API_BASE_URL=http://localhost:5297
```

Default: `http://localhost:5297`

## Usage Example

### Agent Request
```
POST /suggestion
{
  "message": "What trading suggestions do you have for customer 1?"
}
```

### Agent Workflow
1. Agent receives message
2. Calls `get_customer` tool with id=1
3. Calls `get_customer_portfolios` tool with customerId=1
4. Calls `get_customer_preferences` tool with customerId=1
5. Calls `get_all_research_articles` for market insights
6. Generates personalized suggestions based on data

## Implementation Notes

- All tools use async/await pattern
- HTTP client is shared and configured with base URL
- Tool invocations are handled automatically by BaseAgent
- JSON serialization/deserialization handled transparently
- Error responses from API are returned as-is to agent
