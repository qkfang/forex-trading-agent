using Azure.Monitor.OpenTelemetry.AspNetCore;
using FxWebUI.Models;
using FxWebUI.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenTelemetry().UseAzureMonitor();

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddControllers();
builder.Services.AddHttpClient();
builder.Services.AddSingleton<FxDataService>();

builder.Services.AddMcpServer()
    .WithHttpTransport(options => { options.Stateless = true; })
    .WithToolsFromAssembly();

// Add CORS
builder.Services.AddCors();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

// Add CORS for MCP access
app.UseCors(policy => policy
    .AllowAnyOrigin()
    .AllowAnyMethod()
    .AllowAnyHeader());

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

// Aurora quote feed – returns bid/ask for the requested pair
app.MapGet("/api/quote/{pair}", async (string pair, FxDataService fxData) =>
{
    var fx = await fxData.GetCurrentFxRate();
    var mid = fx?.Rate ?? 0.6550m;
    return Results.Ok(new { bid = mid - 0.0002m, ask = mid + 0.0002m, pair, timestamp = DateTime.UtcNow });
});

// Receive a settled trade from Broker Back-Office
app.MapPost("/api/trades", (Transaction transaction, FxDataService fxData) =>
{
    var settled = fxData.AddTransaction(transaction);
    return Results.Ok(new { settled = true, id = settled.Id });
});

// Expose transaction history as JSON (used by FX Agent)
app.MapGet("/api/trades", (FxDataService fxData) =>
    Results.Ok(fxData.GetTransactions()));

app.MapControllers();
app.MapRazorPages();
app.MapMcp("/mcp");
app.MapGet("/mcp", () => Results.Ok("MCP endpoint active. Use POST for JSON-RPC."));

app.Run();
