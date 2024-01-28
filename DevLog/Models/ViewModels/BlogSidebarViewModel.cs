namespace DevLog.Models.ViewModels
{
    public class BlogSidebarViewModel
    {
        public IEnumerable<PostViewModel> PostViewModels { get; set; } = new List<PostViewModel>();
        public IEnumerable<PostCategoryViewModel> PostCategoryViewModels { get; set; } = new List<PostCategoryViewModel>();
        public IEnumerable<string> PostTags { get; set; } = new List<string>();
    }
}
