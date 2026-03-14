using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using FxWebNews.Models;
using FxWebNews.Services;

namespace FxWebNews.Pages.Admin
{
    public class PublishModel : PageModel
    {
        private readonly NewsService _newsService;
        private readonly NewsPublishService _publishService;

        public NewsArticle? Article { get; set; }

        [TempData]
        public string? Message { get; set; }

        [TempData]
        public string? MessageType { get; set; }

        public PublishModel(NewsService newsService, NewsPublishService publishService)
        {
            _newsService = newsService;
            _publishService = publishService;
        }

        public IActionResult OnGet(int id)
        {
            Article = _newsService.GetNewsById(id);
            if (Article == null)
            {
                return RedirectToPage("/Admin/Index");
            }
            return Page();
        }

        public async Task<IActionResult> OnPostPublishAsync(int id)
        {
            var article = _newsService.GetNewsById(id);
            if (article == null)
            {
                return RedirectToPage("/Admin/Index");
            }

            article.IsPublished = true;
            article.PublishedAt = DateTime.UtcNow;
            _newsService.UpdateNews(article);

            var (success, pushMessage) = await _publishService.PushArticleAsync(article);

            if (success)
            {
                Message = $"Article \"{article.Title}\" published successfully! {pushMessage}";
                MessageType = "success";
            }
            else
            {
                Message = $"Article \"{article.Title}\" published locally, but external push failed: {pushMessage}";
                MessageType = "warning";
            }

            return RedirectToPage("/Admin/Index");
        }

        public IActionResult OnPostUnpublish(int id)
        {
            var article = _newsService.GetNewsById(id);
            if (article == null)
            {
                return RedirectToPage("/Admin/Index");
            }

            article.IsPublished = false;
            article.PublishedAt = null;
            _newsService.UpdateNews(article);

            Message = $"Article \"{article.Title}\" moved back to Draft.";
            MessageType = "info";

            return RedirectToPage("/Admin/Index");
        }
    }
}
