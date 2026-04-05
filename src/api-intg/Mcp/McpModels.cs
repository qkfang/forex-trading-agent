namespace FxIntegrationApi.Mcp;

public class McpRequest
{
    public string Jsonrpc { get; set; } = "2.0";
    public string Method { get; set; } = string.Empty;
    public object? Params { get; set; }
    public string? Id { get; set; }
}

public class McpResponse
{
    public string Jsonrpc { get; set; } = "2.0";
    public object? Result { get; set; }
    public McpError? Error { get; set; }
    public string? Id { get; set; }
}

public class McpError
{
    public int Code { get; set; }
    public string Message { get; set; } = string.Empty;
    public object? Data { get; set; }
}

public class McpToolCall
{
    public string Name { get; set; } = string.Empty;
    public Dictionary<string, object>? Arguments { get; set; }
}

public class McpTool
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public object InputSchema { get; set; } = new { };
}

public class McpListToolsResult
{
    public List<McpTool> Tools { get; set; } = new();
}

public class McpCallToolResult
{
    public List<McpContent> Content { get; set; } = new();
    public bool IsError { get; set; }
}

public class McpContent
{
    public string Type { get; set; } = "text";
    public string Text { get; set; } = string.Empty;
}
