using DevLog.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace DevLog.ViewComponents
{
    public class BlogPostCardViewComponent : ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync(PostViewModel postViewModel)
        {
            return await Task.FromResult(View(postViewModel));
        }
    }
}
