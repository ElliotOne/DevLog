namespace DevLog.Core.Domain
{
    public class Post : IEntity
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Body { get; set; } = string.Empty;
        public DateTime CreateDate { get; set; }
        public DateTime LastEditDate { get; set; }
        public string? Tags { get; set; }
        public int? PostCategoryId { get; set; }
        public PostCategory? PostCategory { get; set; }
        public int UserId { get; set; }
        public User User { get; set; } = null!;
        public bool IsCommentsOn { get; set; }
        public string? ImageVirtualPath { get; set; }
        public ICollection<PostComment> PostComments { get; set; } = new List<PostComment>();
    }
}
