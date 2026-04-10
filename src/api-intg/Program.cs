using Azure.Monitor.OpenTelemetry.AspNetCore;
using FxIntegrationApi.Data;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenTelemetry().UseAzureMonitor();

builder.Services.AddControllers()
    .AddJsonOptions(o => o.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<FxDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("FxDatabase")));

builder.Services.AddMcpServer()
    .WithHttpTransport(options => { options.Stateless = true; })
    .WithToolsFromAssembly();

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

// Normalize Accept header for MCP requests from Foundry agent server
app.Use(async (context, next) =>
{
    if (context.Request.Path.StartsWithSegments("/mcp"))
    {
        var accept = context.Request.Headers.Accept.ToString();
        if (string.IsNullOrEmpty(accept) || !accept.Contains("text/event-stream"))
        {
            context.Request.Headers.Accept = "application/json, text/event-stream";
        }
    }
    await next();
});

app.UseHttpsRedirection();
app.MapControllers();
app.MapMcp("/mcp");

app.Run();
