using FxWebNews.Models;
using System.Text;
using System.Text.Json;

namespace FxWebNews.Services
{
    public class NewsPublishService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _config;
        private readonly ILogger<NewsPublishService> _logger;

        public NewsPublishService(HttpClient httpClient, IConfiguration config, ILogger<NewsPublishService> logger)
        {
            _httpClient = httpClient;
            _config = config;
            _logger = logger;
        }

        /// <summary>
        /// Pushes a published article to the configured external endpoint.
        /// Returns true on success, false if the endpoint is unreachable or returns an error.
        /// </summary>
        public async Task<(bool Success, string Message)> PushArticleAsync(NewsArticle article)
        {
            var endpointUrl = _config["NewsPublish:EndpointUrl"];
            if (string.IsNullOrWhiteSpace(endpointUrl))
            {
                _logger.LogWarning("NewsPublish:EndpointUrl is not configured – skipping external push.");
                return (true, "No external endpoint configured.");
            }

            try
            {
                var payload = new
                {
                    id = article.Id,
                    title = article.Title,
                    summary = article.Summary,
                    content = article.Content,
                    type = article.Type,
                    category = article.Category,
                    author = article.Author,
                    publishedAt = article.PublishedAt
                };

                var json = JsonSerializer.Serialize(payload);
                var request = new HttpRequestMessage(HttpMethod.Post, endpointUrl)
                {
                    Content = new StringContent(json, Encoding.UTF8, "application/json")
                };

                var apiKey = _config["NewsPublish:ApiKey"];
                if (!string.IsNullOrWhiteSpace(apiKey))
                {
                    request.Headers.Add("x-api-key", apiKey);
                }

                var response = await _httpClient.SendAsync(request);

                if (response.IsSuccessStatusCode)
                {
                    _logger.LogInformation("Article {Id} pushed to external endpoint successfully.", article.Id);
                    return (true, "Article pushed to external endpoint successfully.");
                }
                else
                {
                    var body = await response.Content.ReadAsStringAsync();
                    _logger.LogWarning("External endpoint returned {Status} for article {Id}: {Body}",
                        (int)response.StatusCode, article.Id, body);
                    return (false, $"External endpoint returned HTTP {(int)response.StatusCode}.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to push article {Id} to external endpoint.", article.Id);
                return (false, $"Could not reach external endpoint: {ex.Message}");
            }
        }
    }
}
