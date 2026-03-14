using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using FxWebNews.Models;
using FxWebNews.Services;

namespace FxWebNews.Pages.Admin
{
    public class IndexModel : PageModel
    {
        private readonly NewsService _newsService;
        public List<NewsArticle> News { get; set; } = new();

        [TempData]
        public string? Message { get; set; }

        [TempData]
        public string? MessageType { get; set; }

        public IndexModel(NewsService newsService)
        {
            _newsService = newsService;
        }

        public void OnGet()
        {
            News = _newsService.GetAllNews();
        }

        public IActionResult OnPost(string title, string summary, string content, string type, string category, string author)
        {
            if (string.IsNullOrEmpty(title) || string.IsNullOrEmpty(content))
            {
                Message = "Title and content are required.";
                MessageType = "danger";
                return RedirectToPage();
            }

            var article = new NewsArticle
            {
                Title = title,
                Summary = summary ?? string.Empty,
                Content = content,
                Type = type,
                Category = category ?? "FX",
                Author = string.IsNullOrWhiteSpace(author) ? "FX News Team" : author,
                IsPublished = false
            };

            _newsService.AddNews(article);
            Message = $"Article \"{title}\" saved as draft. Go to the article to publish it.";
            MessageType = "info";

            return RedirectToPage();
        }

        public IActionResult OnPostDelete(int id)
        {
            _newsService.DeleteNews(id);
            Message = "Article deleted successfully.";
            MessageType = "success";
            return RedirectToPage();
        }
    }
}

