namespace FxWebPortal.Models;

public class TraderSuggestion
{
    public int Id { get; set; }
    public int TraderId { get; set; }
    public int CustomerId { get; set; }
    public int ResearchArticleId { get; set; }
    public string Reasoning { get; set; } = string.Empty;
    public string RelevanceScore { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }

    public TraderSuggestionCustomer? Customer { get; set; }
    public TraderSuggestionTrader? Trader { get; set; }
    public TraderSuggestionArticle? ResearchArticle { get; set; }
}

public class TraderSuggestionCustomer
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Company { get; set; } = string.Empty;
}

public class TraderSuggestionTrader
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
}

public class TraderSuggestionArticle
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string Tags { get; set; } = string.Empty;
    public string Sentiment { get; set; } = string.Empty;
}
