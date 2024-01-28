namespace DevLog.Core.Domain
{
    public class PostComment : IEntity
    {
        public int Id { get; set; }
        public int? ParentId { get; set; }
        public PostComment? Parent { get; set; }
        public ICollection<PostComment>? Children { get; set; }
        public int UserId { get; set; }
        public User? User { get; set; }
        public int PostId { get; set; }
        public Post Post { get; set; } = null!;
        public string? UserFullName { get; set; }
        public string? Email { get; set; }
        public string Body { get; set; } = string.Empty;
        public string Ip { get; set; } = string.Empty;
        public PostCommentStatus Status { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime LastEditDate { get; set; }
        public bool IsEdited { get; set; }
    }
}
