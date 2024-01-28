using DevLog.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace DevLog.ViewComponents
{
    public class CommentsViewComponent : ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync(PostCommentTreeViewModel commentTree)
        {
            var articleCommentTree = new PostCommentTreeViewModel
            {
                CommentSeed = commentTree.CommentSeed,
                PostCommentViewModels = commentTree.PostCommentViewModels
            };

            return await Task.FromResult(View(articleCommentTree));
        }
    }
}
