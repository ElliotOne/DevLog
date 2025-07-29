using DevLog.Core.Domain;

namespace DevLog.Models.ViewModels
{
    public class PostViewModel : IEntity
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Body { get; set; } = string.Empty;
        public DateTime CreateDate { get; set; }
        public string? Tags { get; set; }
        public int UserId { get; set; }
        public string UserFullName { get; set; } = string.Empty;
        public string UserFilePath { get; set; } = string.Empty;
        public string UserBiography { get; set; } = string.Empty;
        public Guid FilesPathGuid { get; set; }
        public bool IsCommentsOn { get; set; }
        public string ImageVirtualPath { get; set; } = string.Empty;
        public PostCategoryViewModel PostCategoryViewModel { get; set; } = new PostCategoryViewModel();
        public PostCommentFormViewModel PostCommentFormViewModel { get; set; } = new PostCommentFormViewModel();
        public IEnumerable<PostCommentViewModel> PostCommentViewModels { get; set; } = new List<PostCommentViewModel>();
    }
}
