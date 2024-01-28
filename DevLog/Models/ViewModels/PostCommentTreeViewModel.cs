namespace DevLog.Models.ViewModels
{
    public class PostCommentTreeViewModel
    {
        public int? CommentSeed { get; set; }
        public IEnumerable<PostCommentViewModel> PostCommentViewModels { get; set; } = new List<PostCommentViewModel>();
    }
}
