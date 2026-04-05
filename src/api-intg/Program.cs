using FxIntegrationApi.Data;
using FxIntegrationApi.Mcp;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers()
    .AddJsonOptions(o => o.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<FxDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("FxDatabase")));

builder.Services.AddScoped<McpToolService>();
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
    });
});

// Seed command: dotnet run -- --seed
if (args.Contains("--seed"))
{
    var connStr = builder.Configuration.GetConnectionString("FxDatabase");
    using var conn = new SqlConnection(connStr);
    conn.Open();
    var sql = File.ReadAllText("Data/seed.sql");
    using var cmd = new SqlCommand(sql, conn);
    cmd.CommandTimeout = 120;
    cmd.ExecuteNonQuery();
    Console.WriteLine("Seed data loaded.");
    return;
}

var app = builder.Build();

app.UseCors();
app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();
app.MapControllers();

app.MapPost("/mcp", async (McpRequest request, McpToolService mcpService) =>
{
    if (request.Method == "tools/list")
    {
        var tools = mcpService.GetAllTools();
        return Results.Ok(new McpResponse
        {
            Id = request.Id,
            Result = new McpListToolsResult { Tools = tools }
        });
    }
    else if (request.Method == "tools/call")
    {
        var toolCall = System.Text.Json.JsonSerializer.Deserialize<McpToolCall>(
            System.Text.Json.JsonSerializer.Serialize(request.Params));
        
        if (toolCall == null)
        {
            return Results.Ok(new McpResponse
            {
                Id = request.Id,
                Error = new McpError { Code = -32602, Message = "Invalid params" }
            });
        }

        var result = await mcpService.CallToolAsync(toolCall.Name, toolCall.Arguments);
        return Results.Ok(new McpResponse
        {
            Id = request.Id,
            Result = result
        });
    }
    else if (request.Method == "initialize")
    {
        return Results.Ok(new McpResponse
        {
            Id = request.Id,
            Result = new
            {
                protocolVersion = "2024-11-05",
                capabilities = new
                {
                    tools = new { }
                },
                serverInfo = new
                {
                    name = "fx-integration-api",
                    version = "1.0.0"
                }
            }
        });
    }

    return Results.Ok(new McpResponse
    {
        Id = request.Id,
        Error = new McpError { Code = -32601, Message = "Method not found" }
    });
});

app.Run();
