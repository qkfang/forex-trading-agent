using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using FxWebNews.Models;
using FxWebNews.Services;

namespace FxWebNews.Pages.Admin
{
    public class BatchPublishModel : PageModel
    {
        private readonly NewsService _newsService;
        private readonly EventHubPublishService _eventHubService;

        public List<NewsArticle> Articles { get; set; } = new();

        [TempData]
        public string? Message { get; set; }

        [TempData]
        public string? MessageType { get; set; }

        public BatchPublishModel(NewsService newsService, EventHubPublishService eventHubService)
        {
            _newsService = newsService;
            _eventHubService = eventHubService;
        }

        public void OnGet()
        {
            Articles = _newsService.GetAllNews();
        }

        public async Task<IActionResult> OnPostAsync(int[] selectedArticles)
        {
            if (selectedArticles == null || selectedArticles.Length == 0)
            {
                Message = "No articles selected for batch publish.";
                MessageType = "warning";
                return RedirectToPage();
            }

            var articles = new List<NewsArticle>();
            foreach (var id in selectedArticles)
            {
                var article = _newsService.GetNewsById(id);
                if (article != null)
                {
                    articles.Add(article);
                }
            }

            if (articles.Count == 0)
            {
                Message = "None of the selected articles were found.";
                MessageType = "danger";
                return RedirectToPage();
            }

            var (success, message, sentCount) = await _eventHubService.PublishBatchAsync(articles);

            if (success)
            {
                Message = message;
                MessageType = "success";
            }
            else
            {
                Message = message;
                MessageType = "danger";
            }

            return RedirectToPage();
        }
    }
}
