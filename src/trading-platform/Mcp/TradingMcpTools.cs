using FxWebUI.Models;
using FxWebUI.Services;
using ModelContextProtocol.Server;
using System.ComponentModel;
using System.Text.Json;

namespace FxWebUI.Mcp;

[McpServerToolType]
public class TradingMcpTools(FxDataService fxData)
{
    [McpServerTool(Name = "get_transactions"), Description("Get trading transaction history with details of all buy and sell activities.")]
    public Task<string> GetTransactions(
        [Description("Account ID to filter transactions")] string accountId,
        [Description("Maximum number of transactions to return (default 50)")] int limit = 50)
    {
        var transactions = fxData.GetTransactions(accountId).Take(limit);
        return Task.FromResult(JsonSerializer.Serialize(transactions));
    }

    [McpServerTool(Name = "get_fund_summary"), Description("Get fund portfolio summary including total balance, AUD balance, USD balance, and profit/loss.")]
    public Task<string> GetFundSummary(
        [Description("Account ID to retrieve fund summary for")] string accountId)
    {
        return Task.FromResult(JsonSerializer.Serialize(fxData.GetFundSummary(accountId)));
    }

    [McpServerTool(Name = "add_transaction"), Description("Add a new buy or sell trading transaction.")]
    public Task<string> AddTransaction(
        [Description("Account ID associated with the transaction")] string accountId,
        [Description("Transaction type: 'Buy' or 'Sell'")] string type,
        [Description("Currency pair, e.g. 'AUD/USD'")] string currencyPair,
        [Description("Amount to trade")] decimal amount,
        [Description("Exchange rate")] decimal rate)
    {
        var transaction = new Transaction
        {
            AccountId = accountId,
            Type = type,
            CurrencyPair = currencyPair,
            Amount = amount,
            Rate = rate,
            Total = amount * rate,
            DateTime = DateTime.UtcNow
        };
        var result = fxData.AddTransaction(transaction);
        return Task.FromResult(JsonSerializer.Serialize(result));
    }
}
