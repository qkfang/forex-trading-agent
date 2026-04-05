# MCP Server Integration

The FX Integration API now exposes an MCP (Model Context Protocol) server endpoint at `/mcp` while maintaining all existing REST API endpoints.

## Architecture

- **REST API** - All existing controllers remain unchanged at `/api/*` endpoints
- **MCP Server** - New JSON-RPC endpoint at `/mcp` for tool-based agent interaction

## MCP Endpoint

**URL:** `POST http://localhost:5297/mcp`

### Supported Methods

1. **initialize** - Initialize MCP connection
2. **tools/list** - List all available tools
3. **tools/call** - Execute a specific tool

## Usage Examples

### 1. Initialize Connection

```json
POST /mcp
{
  "jsonrpc": "2.0",
  "method": "initialize",
  "id": "1"
}
```

Response:
```json
{
  "jsonrpc": "2.0",
  "id": "1",
  "result": {
    "protocolVersion": "2024-11-05",
    "capabilities": {
      "tools": {}
    },
    "serverInfo": {
      "name": "fx-integration-api",
      "version": "1.0.0"
    }
  }
}
```

### 2. List Available Tools

```json
POST /mcp
{
  "jsonrpc": "2.0",
  "method": "tools/list",
  "id": "2"
}
```

Response includes all 25+ tools for customers, portfolios, traders, research, etc.

### 3. Call a Tool

```json
POST /mcp
{
  "jsonrpc": "2.0",
  "method": "tools/call",
  "params": {
    "name": "get_customer",
    "arguments": {
      "id": 1
    }
  },
  "id": "3"
}
```

Response:
```json
{
  "jsonrpc": "2.0",
  "id": "3",
  "result": {
    "content": [
      {
        "type": "text",
        "text": "{\"id\":1,\"name\":\"...\",\"email\":\"...\"}"
      }
    ],
    "isError": false
  }
}
```

## Available MCP Tools

### Customer Management
- `get_all_customers` - Get all customers with portfolios
- `get_customer` - Get customer by ID
- `create_customer` - Create new customer
- `update_customer` - Update customer info
- `delete_customer` - Delete customer

### Portfolio Management
- `get_customer_portfolios` - Get portfolios for a customer
- `get_portfolio` - Get portfolio by ID
- `create_portfolio` - Create new position
- `update_portfolio` - Update position
- `delete_portfolio` - Delete position

### Trader Management
- `get_all_traders` - Get all traders with feeds
- `get_trader` - Get trader by ID
- `create_trader` - Create new trader

### Research Management
- `get_all_research_articles` - Get all articles
- `get_research_article` - Get article by ID
- `create_research_article` - Create new article
- `get_all_research_drafts` - Get all drafts
- `create_research_draft` - Create new draft
- `get_all_research_patterns` - Get all patterns
- `create_research_pattern` - Create new pattern

### Customer Data
- `get_customer_preferences` - Get trading preferences
- `update_customer_preferences` - Update preferences
- `get_customer_history` - Get trading history

### Trader Data
- `get_trader_news` - Get news feeds
- `get_trader_recommendations` - Get recommendations

## Testing with cURL

```bash
# List all tools
curl -X POST http://localhost:5297/mcp \
  -H "Content-Type: application/json" \
  -d '{
    "jsonrpc": "2.0",
    "method": "tools/list",
    "id": "1"
  }'

# Get customer
curl -X POST http://localhost:5297/mcp \
  -H "Content-Type: application/json" \
  -d '{
    "jsonrpc": "2.0",
    "method": "tools/call",
    "params": {
      "name": "get_customer",
      "arguments": {"id": 1}
    },
    "id": "2"
  }'
```

## REST API Unchanged

All existing REST endpoints continue to work:
- `GET /api/customers`
- `GET /api/customers/{id}`
- `GET /api/portfolios/customer/{customerId}`
- `GET /api/traders`
- `GET /api/researcharticles`
- etc.

## Configuration

CORS is enabled for all origins in development. The MCP endpoint requires no authentication in this implementation.
