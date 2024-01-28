using DevLog.Core.Domain;

namespace DevLog.Models.ViewModels
{
    public class PostCommentViewModel : IEntity
    {
        public int Id { get; set; }
        public int? ParentId { get; set; }
        public string Body { get; set; } = string.Empty;
        public DateTime CreateDate { get; set; }
        public bool IsEdited { get; set; }
        public string? UserFullName { get; set; }
    }
}
