using DevLog.Models.Parameters;
using DevLog.Models.Shared.Pagination;

namespace DevLog.Models.ViewModels
{
    public class PostsViewModel
    {
        public IEnumerable<PostViewModel> PostViewModels { get; set; } = new List<PostViewModel>();
        public Pager Pager { get; set; } = new Pager(default, default, default, default);
        public string? SearchString { get; set; }
        public int? PostCategoryId { get; set; }
        public string? PostTag { get; set; }
        public PostSortFilterType PostSortFilterType { get; set; }
    }
}
