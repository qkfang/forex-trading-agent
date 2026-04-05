using System.Text;
using System.Text.Json;

namespace FxAgent;

public static class McpTools
{
    private static readonly HttpClient _http = new()
    {
        BaseAddress = new Uri(Environment.GetEnvironmentVariable("API_BASE_URL") ?? "http://localhost:5297")
    };

    private static readonly HttpClient _mcpHttp = new()
    {
        BaseAddress = new Uri(Environment.GetEnvironmentVariable("MCP_BROKER_URL") ?? "http://localhost:5269")
    };

    public static class Customers
    {
        public static async Task<string> GetAll()
        {
            var response = await _http.GetAsync("/api/customers");
            return await response.Content.ReadAsStringAsync();
        }

        public static async Task<string> Get(int id)
        {
            var response = await _http.GetAsync($"/api/customers/{id}");
            return await response.Content.ReadAsStringAsync();
        }

        public static async Task<string> Create(string name, string email, string phone, string company)
        {
            var data = new { Name = name, Email = email, Phone = phone, Company = company };
            var content = new StringContent(JsonSerializer.Serialize(data), Encoding.UTF8, "application/json");
            var response = await _http.PostAsync("/api/customers", content);
            return await response.Content.ReadAsStringAsync();
        }

        public static async Task<string> Update(int id, string name, string email, string phone, string company)
        {
            var data = new { Id = id, Name = name, Email = email, Phone = phone, Company = company };
            var content = new StringContent(JsonSerializer.Serialize(data), Encoding.UTF8, "application/json");
            var response = await _http.PutAsync($"/api/customers/{id}", content);
            return response.IsSuccessStatusCode ? "Updated successfully" : await response.Content.ReadAsStringAsync();
        }

        public static async Task<string> Delete(int id)
        {
            var response = await _http.DeleteAsync($"/api/customers/{id}");
            return response.IsSuccessStatusCode ? "Deleted successfully" : await response.Content.ReadAsStringAsync();
        }
    }

    public static class Portfolios
    {
        public static async Task<string> GetByCustomer(int customerId)
        {
            var response = await _http.GetAsync($"/api/portfolios/customer/{customerId}");
            return await response.Content.ReadAsStringAsync();
        }

        public static async Task<string> Get(int id)
        {
            var response = await _http.GetAsync($"/api/portfolios/{id}");
            return await response.Content.ReadAsStringAsync();
        }

        public static async Task<string> Create(int customerId, string currencyPair, string direction, decimal amount, decimal entryRate, string status = "Open")
        {
            var data = new { CustomerId = customerId, CurrencyPair = currencyPair, Direction = direction, Amount = amount, EntryRate = entryRate, Status = status };
            var content = new StringContent(JsonSerializer.Serialize(data), Encoding.UTF8, "application/json");
            var response = await _http.PostAsync("/api/portfolios", content);
            return await response.Content.ReadAsStringAsync();
        }

        public static async Task<string> Update(int id, int customerId, string currencyPair, string direction, decimal amount, decimal entryRate, string status)
        {
            var data = new { Id = id, CustomerId = customerId, CurrencyPair = currencyPair, Direction = direction, Amount = amount, EntryRate = entryRate, Status = status };
            var content = new StringContent(JsonSerializer.Serialize(data), Encoding.UTF8, "application/json");
            var response = await _http.PutAsync($"/api/portfolios/{id}", content);
            return response.IsSuccessStatusCode ? "Updated successfully" : await response.Content.ReadAsStringAsync();
        }

        public static async Task<string> Delete(int id)
        {
            var response = await _http.DeleteAsync($"/api/portfolios/{id}");
            return response.IsSuccessStatusCode ? "Deleted successfully" : await response.Content.ReadAsStringAsync();
        }
    }

    public static class Traders
    {
        public static async Task<string> GetAll()
        {
            var response = await _http.GetAsync("/api/traders");
            return await response.Content.ReadAsStringAsync();
        }

        public static async Task<string> Get(int id)
        {
            var response = await _http.GetAsync($"/api/traders/{id}");
            return await response.Content.ReadAsStringAsync();
        }

        public static async Task<string> Create(string name, string email, string expertise, string yearsActive)
        {
            var data = new { Name = name, Email = email, Expertise = expertise, YearsActive = yearsActive };
            var content = new StringContent(JsonSerializer.Serialize(data), Encoding.UTF8, "application/json");
            var response = await _http.PostAsync("/api/traders", content);
            return await response.Content.ReadAsStringAsync();
        }

        public static async Task<string> Update(int id, string name, string email, string expertise, string yearsActive)
        {
            var data = new { Id = id, Name = name, Email = email, Expertise = expertise, YearsActive = yearsActive };
            var content = new StringContent(JsonSerializer.Serialize(data), Encoding.UTF8, "application/json");
            var response = await _http.PutAsync($"/api/traders/{id}", content);
            return response.IsSuccessStatusCode ? "Updated successfully" : await response.Content.ReadAsStringAsync();
        }

        public static async Task<string> Delete(int id)
        {
            var response = await _http.DeleteAsync($"/api/traders/{id}");
            return response.IsSuccessStatusCode ? "Deleted successfully" : await response.Content.ReadAsStringAsync();
        }
    }

    public static class ResearchArticles
    {
        public static async Task<string> GetAll()
        {
            var response = await _http.GetAsync("/api/researcharticles");
            return await response.Content.ReadAsStringAsync();
        }

        public static async Task<string> Get(int id)
        {
            var response = await _http.GetAsync($"/api/researcharticles/{id}");
            return await response.Content.ReadAsStringAsync();
        }

        public static async Task<string> Create(string title, string content, string currencyPair, string analysis)
        {
            var data = new { Title = title, Content = content, CurrencyPair = currencyPair, Analysis = analysis };
            var content = new StringContent(JsonSerializer.Serialize(data), Encoding.UTF8, "application/json");
            var response = await _http.PostAsync("/api/researcharticles", content);
            return await response.Content.ReadAsStringAsync();
        }

        public static async Task<string> Update(int id, string title, string content, string currencyPair, string analysis)
        {
            var data = new { Id = id, Title = title, Content = content, CurrencyPair = currencyPair, Analysis = analysis };
            var content = new StringContent(JsonSerializer.Serialize(data), Encoding.UTF8, "application/json");
            var response = await _http.PutAsync($"/api/researcharticles/{id}", content);
            return response.IsSuccessStatusCode ? "Updated successfully" : await response.Content.ReadAsStringAsync();
        }

        public static async Task<string> Delete(int id)
        {
            var response = await _http.DeleteAsync($"/api/researcharticles/{id}");
            return response.IsSuccessStatusCode ? "Deleted successfully" : await response.Content.ReadAsStringAsync();
        }
    }

    public static class CustomerPreferences
    {
        public static async Task<string> GetByCustomer(int customerId)
        {
            var response = await _http.GetAsync($"/api/customerpreferences/customer/{customerId}");
            return await response.Content.ReadAsStringAsync();
        }

        public static async Task<string> Get(int id)
        {
            var response = await _http.GetAsync($"/api/customerpreferences/{id}");
            return await response.Content.ReadAsStringAsync();
        }

        public static async Task<string> Create(int customerId, string riskLevel, string preferredPairs, string tradingObjective)
        {
            var data = new { CustomerId = customerId, RiskLevel = riskLevel, PreferredPairs = preferredPairs, TradingObjective = tradingObjective };
            var content = new StringContent(JsonSerializer.Serialize(data), Encoding.UTF8, "application/json");
            var response = await _http.PostAsync("/api/customerpreferences", content);
            return await response.Content.ReadAsStringAsync();
        }

        public static async Task<string> Update(int id, int customerId, string riskLevel, string preferredPairs, string tradingObjective)
        {
            var data = new { Id = id, CustomerId = customerId, RiskLevel = riskLevel, PreferredPairs = preferredPairs, TradingObjective = tradingObjective };
            var content = new StringContent(JsonSerializer.Serialize(data), Encoding.UTF8, "application/json");
            var response = await _http.PutAsync($"/api/customerpreferences/{id}", content);
            return response.IsSuccessStatusCode ? "Updated successfully" : await response.Content.ReadAsStringAsync();
        }
    }

    public static class CustomerHistories
    {
        public static async Task<string> GetByCustomer(int customerId)
        {
            var response = await _http.GetAsync($"/api/customerhistories/customer/{customerId}");
            return await response.Content.ReadAsStringAsync();
        }

        public static async Task<string> Get(int id)
        {
            var response = await _http.GetAsync($"/api/customerhistories/{id}");
            return await response.Content.ReadAsStringAsync();
        }
    }

    public static class ResearchDrafts
    {
        public static async Task<string> GetAll()
        {
            var response = await _http.GetAsync("/api/researchdrafts");
            return await response.Content.ReadAsStringAsync();
        }

        public static async Task<string> Get(int id)
        {
            var response = await _http.GetAsync($"/api/researchdrafts/{id}");
            return await response.Content.ReadAsStringAsync();
        }

        public static async Task<string> Create(string title, string content, string status)
        {
            var data = new { Title = title, Content = content, Status = status };
            var content = new StringContent(JsonSerializer.Serialize(data), Encoding.UTF8, "application/json");
            var response = await _http.PostAsync("/api/researchdrafts", content);
            return await response.Content.ReadAsStringAsync();
        }

        public static async Task<string> Update(int id, string title, string content, string status)
        {
            var data = new { Id = id, Title = title, Content = content, Status = status };
            var content = new StringContent(JsonSerializer.Serialize(data), Encoding.UTF8, "application/json");
            var response = await _http.PutAsync($"/api/researchdrafts/{id}", content);
            return response.IsSuccessStatusCode ? "Updated successfully" : await response.Content.ReadAsStringAsync();
        }

        public static async Task<string> Delete(int id)
        {
            var response = await _http.DeleteAsync($"/api/researchdrafts/{id}");
            return response.IsSuccessStatusCode ? "Deleted successfully" : await response.Content.ReadAsStringAsync();
        }
    }

    public static class ResearchPatterns
    {
        public static async Task<string> GetAll()
        {
            var response = await _http.GetAsync("/api/researchpatterns");
            return await response.Content.ReadAsStringAsync();
        }

        public static async Task<string> Get(int id)
        {
            var response = await _http.GetAsync($"/api/researchpatterns/{id}");
            return await response.Content.ReadAsStringAsync();
        }

        public static async Task<string> Create(string patternName, string description, string currencyPair)
        {
            var data = new { PatternName = patternName, Description = description, CurrencyPair = currencyPair };
            var content = new StringContent(JsonSerializer.Serialize(data), Encoding.UTF8, "application/json");
            var response = await _http.PostAsync("/api/researchpatterns", content);
            return await response.Content.ReadAsStringAsync();
        }

        public static async Task<string> Update(int id, string patternName, string description, string currencyPair)
        {
            var data = new { Id = id, PatternName = patternName, Description = description, CurrencyPair = currencyPair };
            var content = new StringContent(JsonSerializer.Serialize(data), Encoding.UTF8, "application/json");
            var response = await _http.PutAsync($"/api/researchpatterns/{id}", content);
            return response.IsSuccessStatusCode ? "Updated successfully" : await response.Content.ReadAsStringAsync();
        }

        public static async Task<string> Delete(int id)
        {
            var response = await _http.DeleteAsync($"/api/researchpatterns/{id}");
            return response.IsSuccessStatusCode ? "Deleted successfully" : await response.Content.ReadAsStringAsync();
        }
    }

    public static class TraderNewsFeeds
    {
        public static async Task<string> GetByTrader(int traderId)
        {
            var response = await _http.GetAsync($"/api/tradernewsfeeds/trader/{traderId}");
            return await response.Content.ReadAsStringAsync();
        }

        public static async Task<string> Get(int id)
        {
            var response = await _http.GetAsync($"/api/tradernewsfeeds/{id}");
            return await response.Content.ReadAsStringAsync();
        }
    }

    public static class TraderRecommendations
    {
        public static async Task<string> GetByTrader(int traderId)
        {
            var response = await _http.GetAsync($"/api/traderrecommendations/trader/{traderId}");
            return await response.Content.ReadAsStringAsync();
        }

        public static async Task<string> Get(int id)
        {
            var response = await _http.GetAsync($"/api/traderrecommendations/{id}");
            return await response.Content.ReadAsStringAsync();
        }
    }

    public static class TradingMcp
    {
        public static async Task<string> GetQuote()
        {
            var response = await _mcpHttp.PostAsync("/mcp/call",
                new StringContent(JsonSerializer.Serialize(new { tool = "fx_quote", parameters = new { } }), Encoding.UTF8, "application/json"));
            return await response.Content.ReadAsStringAsync();
        }

        public static async Task<string> ExecuteBuy(decimal amount)
        {
            var response = await _mcpHttp.PostAsync("/mcp/call",
                new StringContent(JsonSerializer.Serialize(new { tool = "fx_buy", parameters = new { amount } }), Encoding.UTF8, "application/json"));
            return await response.Content.ReadAsStringAsync();
        }

        public static async Task<string> ExecuteSell(decimal amount)
        {
            var response = await _mcpHttp.PostAsync("/mcp/call",
                new StringContent(JsonSerializer.Serialize(new { tool = "fx_sell", parameters = new { amount } }), Encoding.UTF8, "application/json"));
            return await response.Content.ReadAsStringAsync();
        }

        public static async Task<string> GetHistory(int bars = 50)
        {
            var response = await _mcpHttp.PostAsync("/mcp/call",
                new StringContent(JsonSerializer.Serialize(new { tool = "fx_history", parameters = new { bars } }), Encoding.UTF8, "application/json"));
            return await response.Content.ReadAsStringAsync();
        }

        public static async Task<string> GetMarketStatus()
        {
            var response = await _mcpHttp.PostAsync("/mcp/call",
                new StringContent(JsonSerializer.Serialize(new { tool = "fx_market_status", parameters = new { } }), Encoding.UTF8, "application/json"));
            return await response.Content.ReadAsStringAsync();
        }

        public static async Task<string> SetTrend(string direction, int strength = 70)
        {
            var response = await _mcpHttp.PostAsync("/mcp/call",
                new StringContent(JsonSerializer.Serialize(new { tool = "fx_set_trend", parameters = new { direction, strength } }), Encoding.UTF8, "application/json"));
            return await response.Content.ReadAsStringAsync();
        }
    }

    public static class AzureSearch
    {
        private static readonly string _endpoint = Environment.GetEnvironmentVariable("AZURE_SEARCH_ENDPOINT") ?? "";

        public static async Task<string> SearchWeb(string query, int count = 10)
        {
            if (string.IsNullOrEmpty(_endpoint))
                return JsonSerializer.Serialize(new { error = "Azure Search endpoint not configured" });

            return JsonSerializer.Serialize(new
            {
                message = "Web search via Azure AI Search",
                query,
                count,
                endpoint = _endpoint,
                note = "Configure Azure AI Search index for web grounding"
            });
        }

        public static async Task<string> SearchNews(string query, int count = 10)
        {
            if (string.IsNullOrEmpty(_endpoint))
                return JsonSerializer.Serialize(new { error = "Azure Search endpoint not configured" });

            return JsonSerializer.Serialize(new
            {
                message = "News search via Azure AI Search",
                query,
                count,
                endpoint = _endpoint,
                note = "Configure Azure AI Search index for news grounding"
            });
        }
    }
}
