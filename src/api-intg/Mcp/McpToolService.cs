using FxIntegrationApi.Data;
using FxIntegrationApi.Models;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace FxIntegrationApi.Mcp;

public class McpToolService
{
    private readonly FxDbContext _db;

    public McpToolService(FxDbContext db)
    {
        _db = db;
    }

    public List<McpTool> GetAllTools()
    {
        return new List<McpTool>
        {
            new McpTool { Name = "get_all_customers", Description = "Get all customers with their portfolios", InputSchema = new { type = "object", properties = new { } } },
            new McpTool { Name = "get_customer", Description = "Get customer by ID", InputSchema = new { type = "object", properties = new { id = new { type = "integer", description = "Customer ID" } }, required = new[] { "id" } } },
            new McpTool { Name = "create_customer", Description = "Create new customer", InputSchema = new { type = "object", properties = new { name = new { type = "string" }, email = new { type = "string" }, phone = new { type = "string" }, company = new { type = "string" } }, required = new[] { "name", "email", "phone", "company" } } },
            new McpTool { Name = "update_customer", Description = "Update customer information", InputSchema = new { type = "object", properties = new { id = new { type = "integer" }, name = new { type = "string" }, email = new { type = "string" }, phone = new { type = "string" }, company = new { type = "string" } }, required = new[] { "id", "name", "email", "phone", "company" } } },
            new McpTool { Name = "delete_customer", Description = "Delete customer", InputSchema = new { type = "object", properties = new { id = new { type = "integer" } }, required = new[] { "id" } } },

            new McpTool { Name = "get_customer_portfolios", Description = "Get all portfolios for a customer", InputSchema = new { type = "object", properties = new { customerId = new { type = "integer" } }, required = new[] { "customerId" } } },
            new McpTool { Name = "get_portfolio", Description = "Get portfolio by ID", InputSchema = new { type = "object", properties = new { id = new { type = "integer" } }, required = new[] { "id" } } },
            new McpTool { Name = "create_portfolio", Description = "Create new portfolio position", InputSchema = new { type = "object", properties = new { customerId = new { type = "integer" }, currencyPair = new { type = "string" }, direction = new { type = "string" }, amount = new { type = "number" }, entryRate = new { type = "number" }, status = new { type = "string" } }, required = new[] { "customerId", "currencyPair", "direction", "amount", "entryRate" } } },
            new McpTool { Name = "update_portfolio", Description = "Update portfolio position", InputSchema = new { type = "object", properties = new { id = new { type = "integer" }, customerId = new { type = "integer" }, currencyPair = new { type = "string" }, direction = new { type = "string" }, amount = new { type = "number" }, entryRate = new { type = "number" }, status = new { type = "string" } }, required = new[] { "id", "customerId", "currencyPair", "direction", "amount", "entryRate", "status" } } },
            new McpTool { Name = "delete_portfolio", Description = "Delete portfolio position", InputSchema = new { type = "object", properties = new { id = new { type = "integer" } }, required = new[] { "id" } } },

            new McpTool { Name = "get_all_traders", Description = "Get all traders with recommendations and feeds", InputSchema = new { type = "object", properties = new { } } },
            new McpTool { Name = "get_trader", Description = "Get trader by ID", InputSchema = new { type = "object", properties = new { id = new { type = "integer" } }, required = new[] { "id" } } },
            new McpTool { Name = "create_trader", Description = "Create new trader", InputSchema = new { type = "object", properties = new { name = new { type = "string" }, email = new { type = "string" }, desk = new { type = "string" }, specialization = new { type = "string" }, region = new { type = "string" } }, required = new[] { "name", "email", "desk", "specialization", "region" } } },

            new McpTool { Name = "get_all_research_articles", Description = "Get all research articles", InputSchema = new { type = "object", properties = new { } } },
            new McpTool { Name = "get_research_article", Description = "Get research article by ID", InputSchema = new { type = "object", properties = new { id = new { type = "integer" } }, required = new[] { "id" } } },
            new McpTool { Name = "create_research_article", Description = "Create new research article", InputSchema = new { type = "object", properties = new { title = new { type = "string" }, summary = new { type = "string" }, content = new { type = "string" }, category = new { type = "string" }, author = new { type = "string" }, sentiment = new { type = "string" } }, required = new[] { "title", "content" } } },

            new McpTool { Name = "get_customer_preferences", Description = "Get customer trading preferences", InputSchema = new { type = "object", properties = new { customerId = new { type = "integer" } }, required = new[] { "customerId" } } },
            new McpTool { Name = "update_customer_preferences", Description = "Update customer preferences", InputSchema = new { type = "object", properties = new { id = new { type = "integer" }, customerId = new { type = "integer" }, preferredCurrencyPairs = new { type = "string" }, riskTolerance = new { type = "string" }, tradingStyle = new { type = "string" }, tradingObjective = new { type = "string" } }, required = new[] { "id", "customerId" } } },

            new McpTool { Name = "get_customer_history", Description = "Get customer trading history", InputSchema = new { type = "object", properties = new { customerId = new { type = "integer" } }, required = new[] { "customerId" } } },

            new McpTool { Name = "get_all_research_drafts", Description = "Get all research drafts", InputSchema = new { type = "object", properties = new { } } },
            new McpTool { Name = "create_research_draft", Description = "Create new research draft", InputSchema = new { type = "object", properties = new { title = new { type = "string" }, content = new { type = "string" }, author = new { type = "string" }, category = new { type = "string" }, status = new { type = "string" } }, required = new[] { "title", "content" } } },

            new McpTool { Name = "get_all_research_patterns", Description = "Get all identified trading patterns", InputSchema = new { type = "object", properties = new { } } },
            new McpTool { Name = "create_research_pattern", Description = "Create new pattern observation", InputSchema = new { type = "object", properties = new { currencyPair = new { type = "string" }, patternName = new { type = "string" }, timeframe = new { type = "string" }, direction = new { type = "string" }, description = new { type = "string" }, detectedBy = new { type = "string" } }, required = new[] { "currencyPair", "patternName" } } },

            new McpTool { Name = "get_trader_news", Description = "Get news feeds for a trader", InputSchema = new { type = "object", properties = new { traderId = new { type = "integer" } }, required = new[] { "traderId" } } },
            new McpTool { Name = "get_trader_recommendations", Description = "Get trader recommendations", InputSchema = new { type = "object", properties = new { traderId = new { type = "integer" } }, required = new[] { "traderId" } } },
        };
    }

    public async Task<McpCallToolResult> CallToolAsync(string toolName, Dictionary<string, object>? arguments)
    {
        try
        {
            var result = toolName switch
            {
                "get_all_customers" => await GetAllCustomers(),
                "get_customer" => await GetCustomer(GetInt(arguments, "id")),
                "create_customer" => await CreateCustomer(GetString(arguments, "name"), GetString(arguments, "email"), GetString(arguments, "phone"), GetString(arguments, "company")),
                "update_customer" => await UpdateCustomer(GetInt(arguments, "id"), GetString(arguments, "name"), GetString(arguments, "email"), GetString(arguments, "phone"), GetString(arguments, "company")),
                "delete_customer" => await DeleteCustomer(GetInt(arguments, "id")),

                "get_customer_portfolios" => await GetCustomerPortfolios(GetInt(arguments, "customerId")),
                "get_portfolio" => await GetPortfolio(GetInt(arguments, "id")),
                "create_portfolio" => await CreatePortfolio(GetInt(arguments, "customerId"), GetString(arguments, "currencyPair"), GetString(arguments, "direction"), GetDecimal(arguments, "amount"), GetDecimal(arguments, "entryRate"), GetString(arguments, "status", "Open")),
                "update_portfolio" => await UpdatePortfolio(GetInt(arguments, "id"), GetInt(arguments, "customerId"), GetString(arguments, "currencyPair"), GetString(arguments, "direction"), GetDecimal(arguments, "amount"), GetDecimal(arguments, "entryRate"), GetString(arguments, "status")),
                "delete_portfolio" => await DeletePortfolio(GetInt(arguments, "id")),

                "get_all_traders" => await GetAllTraders(),
                "get_trader" => await GetTrader(GetInt(arguments, "id")),
                "create_trader" => await CreateTrader(GetString(arguments, "name"), GetString(arguments, "email"), GetString(arguments, "desk"), GetString(arguments, "specialization"), GetString(arguments, "region")),

                "get_all_research_articles" => await GetAllResearchArticles(),
                "get_research_article" => await GetResearchArticle(GetInt(arguments, "id")),
                "create_research_article" => await CreateResearchArticle(GetString(arguments, "title"), GetString(arguments, "summary"), GetString(arguments, "content"), GetString(arguments, "category"), GetString(arguments, "author"), GetString(arguments, "sentiment", "Neutral")),

                "get_customer_preferences" => await GetCustomerPreferences(GetInt(arguments, "customerId")),
                "update_customer_preferences" => await UpdateCustomerPreferences(GetInt(arguments, "id"), GetInt(arguments, "customerId"), GetString(arguments, "preferredCurrencyPairs"), GetString(arguments, "riskTolerance"), GetString(arguments, "tradingStyle"), GetString(arguments, "tradingObjective")),

                "get_customer_history" => await GetCustomerHistory(GetInt(arguments, "customerId")),

                "get_all_research_drafts" => await GetAllResearchDrafts(),
                "create_research_draft" => await CreateResearchDraft(GetString(arguments, "title"), GetString(arguments, "content"), GetString(arguments, "author"), GetString(arguments, "category"), GetString(arguments, "status", "InProgress")),

                "get_all_research_patterns" => await GetAllResearchPatterns(),
                "create_research_pattern" => await CreateResearchPattern(GetString(arguments, "currencyPair"), GetString(arguments, "patternName"), GetString(arguments, "timeframe"), GetString(arguments, "direction"), GetString(arguments, "description"), GetString(arguments, "detectedBy")),

                "get_trader_news" => await GetTraderNews(GetInt(arguments, "traderId")),
                "get_trader_recommendations" => await GetTraderRecommendations(GetInt(arguments, "traderId")),

                _ => JsonSerializer.Serialize(new { error = "Unknown tool" })
            };

            return new McpCallToolResult
            {
                Content = new List<McpContent> { new McpContent { Type = "text", Text = result } }
            };
        }
        catch (Exception ex)
        {
            return new McpCallToolResult
            {
                Content = new List<McpContent> { new McpContent { Type = "text", Text = $"Error: {ex.Message}" } },
                IsError = true
            };
        }
    }

    private async Task<string> GetAllCustomers()
    {
        var customers = await _db.Customers.Include(c => c.Portfolios).ToListAsync();
        return JsonSerializer.Serialize(customers);
    }

    private async Task<string> GetCustomer(int id)
    {
        var customer = await _db.Customers.Include(c => c.Portfolios).FirstOrDefaultAsync(c => c.Id == id);
        return customer is null ? JsonSerializer.Serialize(new { error = "Not found" }) : JsonSerializer.Serialize(customer);
    }

    private async Task<string> CreateCustomer(string name, string email, string phone, string company)
    {
        var customer = new Customer { Name = name, Email = email, Phone = phone, Company = company };
        _db.Customers.Add(customer);
        await _db.SaveChangesAsync();
        return JsonSerializer.Serialize(customer);
    }

    private async Task<string> UpdateCustomer(int id, string name, string email, string phone, string company)
    {
        var customer = await _db.Customers.FindAsync(id);
        if (customer is null) return JsonSerializer.Serialize(new { error = "Not found" });
        customer.Name = name;
        customer.Email = email;
        customer.Phone = phone;
        customer.Company = company;
        await _db.SaveChangesAsync();
        return JsonSerializer.Serialize(new { success = true });
    }

    private async Task<string> DeleteCustomer(int id)
    {
        var customer = await _db.Customers.FindAsync(id);
        if (customer is null) return JsonSerializer.Serialize(new { error = "Not found" });
        _db.Customers.Remove(customer);
        await _db.SaveChangesAsync();
        return JsonSerializer.Serialize(new { success = true });
    }

    private async Task<string> GetCustomerPortfolios(int customerId)
    {
        var portfolios = await _db.CustomerPortfolios.Where(p => p.CustomerId == customerId).ToListAsync();
        return JsonSerializer.Serialize(portfolios);
    }

    private async Task<string> GetPortfolio(int id)
    {
        var portfolio = await _db.CustomerPortfolios.FindAsync(id);
        return portfolio is null ? JsonSerializer.Serialize(new { error = "Not found" }) : JsonSerializer.Serialize(portfolio);
    }

    private async Task<string> CreatePortfolio(int customerId, string currencyPair, string direction, decimal amount, decimal entryRate, string status)
    {
        var portfolio = new CustomerPortfolio { CustomerId = customerId, CurrencyPair = currencyPair, Direction = direction, Amount = amount, EntryRate = entryRate, Status = status };
        _db.CustomerPortfolios.Add(portfolio);
        await _db.SaveChangesAsync();
        return JsonSerializer.Serialize(portfolio);
    }

    private async Task<string> UpdatePortfolio(int id, int customerId, string currencyPair, string direction, decimal amount, decimal entryRate, string status)
    {
        var portfolio = await _db.CustomerPortfolios.FindAsync(id);
        if (portfolio is null) return JsonSerializer.Serialize(new { error = "Not found" });
        portfolio.CustomerId = customerId;
        portfolio.CurrencyPair = currencyPair;
        portfolio.Direction = direction;
        portfolio.Amount = amount;
        portfolio.EntryRate = entryRate;
        portfolio.Status = status;
        await _db.SaveChangesAsync();
        return JsonSerializer.Serialize(new { success = true });
    }

    private async Task<string> DeletePortfolio(int id)
    {
        var portfolio = await _db.CustomerPortfolios.FindAsync(id);
        if (portfolio is null) return JsonSerializer.Serialize(new { error = "Not found" });
        _db.CustomerPortfolios.Remove(portfolio);
        await _db.SaveChangesAsync();
        return JsonSerializer.Serialize(new { success = true });
    }

    private async Task<string> GetAllTraders()
    {
        var traders = await _db.Traders.Include(t => t.Recommendations).Include(t => t.NewsFeeds).ToListAsync();
        return JsonSerializer.Serialize(traders);
    }

    private async Task<string> GetTrader(int id)
    {
        var trader = await _db.Traders.Include(t => t.Recommendations).Include(t => t.NewsFeeds).FirstOrDefaultAsync(t => t.Id == id);
        return trader is null ? JsonSerializer.Serialize(new { error = "Not found" }) : JsonSerializer.Serialize(trader);
    }

    private async Task<string> CreateTrader(string name, string email, string desk, string specialization, string region)
    {
        var trader = new Trader { Name = name, Email = email, Desk = desk, Specialization = specialization, Region = region, JoinedAt = DateTime.UtcNow };
        _db.Traders.Add(trader);
        await _db.SaveChangesAsync();
        return JsonSerializer.Serialize(trader);
    }

    private async Task<string> GetAllResearchArticles()
    {
        var articles = await _db.ResearchArticles.ToListAsync();
        return JsonSerializer.Serialize(articles);
    }

    private async Task<string> GetResearchArticle(int id)
    {
        var article = await _db.ResearchArticles.FindAsync(id);
        return article is null ? JsonSerializer.Serialize(new { error = "Not found" }) : JsonSerializer.Serialize(article);
    }

    private async Task<string> CreateResearchArticle(string title, string summary, string content, string category, string author, string sentiment)
    {
        var article = new ResearchArticle { Title = title, Summary = summary, Content = content, Category = category, Author = author, Sentiment = sentiment, PublishedDate = DateTime.UtcNow };
        _db.ResearchArticles.Add(article);
        await _db.SaveChangesAsync();
        return JsonSerializer.Serialize(article);
    }

    private async Task<string> GetCustomerPreferences(int customerId)
    {
        var pref = await _db.CustomerPreferences.FirstOrDefaultAsync(p => p.CustomerId == customerId);
        return pref is null ? JsonSerializer.Serialize(new { error = "Not found" }) : JsonSerializer.Serialize(pref);
    }

    private async Task<string> UpdateCustomerPreferences(int id, int customerId, string preferredCurrencyPairs, string riskTolerance, string tradingStyle, string tradingObjective)
    {
        var pref = await _db.CustomerPreferences.FindAsync(id);
        if (pref is null) return JsonSerializer.Serialize(new { error = "Not found" });
        pref.CustomerId = customerId;
        pref.PreferredCurrencyPairs = preferredCurrencyPairs;
        pref.RiskTolerance = riskTolerance;
        pref.TradingStyle = tradingStyle;
        pref.TradingObjective = tradingObjective;
        pref.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();
        return JsonSerializer.Serialize(new { success = true });
    }

    private async Task<string> GetCustomerHistory(int customerId)
    {
        var history = await _db.CustomerHistories.Where(h => h.CustomerId == customerId).ToListAsync();
        return JsonSerializer.Serialize(history);
    }

    private async Task<string> GetAllResearchDrafts()
    {
        var drafts = await _db.ResearchDrafts.ToListAsync();
        return JsonSerializer.Serialize(drafts);
    }

    private async Task<string> CreateResearchDraft(string title, string content, string author, string category, string status)
    {
        var draft = new ResearchDraft { Title = title, Content = content, Author = author, Category = category, Status = status };
        _db.ResearchDrafts.Add(draft);
        await _db.SaveChangesAsync();
        return JsonSerializer.Serialize(draft);
    }

    private async Task<string> GetAllResearchPatterns()
    {
        var patterns = await _db.ResearchPatterns.ToListAsync();
        return JsonSerializer.Serialize(patterns);
    }

    private async Task<string> CreateResearchPattern(string currencyPair, string patternName, string timeframe, string direction, string description, string detectedBy)
    {
        var pattern = new ResearchPattern 
        { 
            CurrencyPair = currencyPair, 
            PatternName = patternName, 
            Timeframe = timeframe, 
            Direction = direction, 
            Description = description, 
            DetectedBy = detectedBy,
            DetectedAt = DateTime.UtcNow
        };
        _db.ResearchPatterns.Add(pattern);
        await _db.SaveChangesAsync();
        return JsonSerializer.Serialize(pattern);
    }

    private async Task<string> GetTraderNews(int traderId)
    {
        var news = await _db.TraderNewsFeeds.Where(n => n.TraderId == traderId).ToListAsync();
        return JsonSerializer.Serialize(news);
    }

    private async Task<string> GetTraderRecommendations(int traderId)
    {
        var recs = await _db.TraderRecommendations.Where(r => r.TraderId == traderId).ToListAsync();
        return JsonSerializer.Serialize(recs);
    }

    private static int GetInt(Dictionary<string, object>? args, string key)
    {
        if (args == null || !args.TryGetValue(key, out var value)) return 0;
        if (value is JsonElement elem) return elem.GetInt32();
        return Convert.ToInt32(value);
    }

    private static string GetString(Dictionary<string, object>? args, string key, string defaultValue = "")
    {
        if (args == null || !args.TryGetValue(key, out var value)) return defaultValue;
        if (value is JsonElement elem) return elem.GetString() ?? defaultValue;
        return value?.ToString() ?? defaultValue;
    }

    private static decimal GetDecimal(Dictionary<string, object>? args, string key)
    {
        if (args == null || !args.TryGetValue(key, out var value)) return 0;
        if (value is JsonElement elem) return elem.GetDecimal();
        return Convert.ToDecimal(value);
    }
}
