namespace DevLog.Core.Domain
{
    public class PostCategory : IEntity
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public DateTime CreateDate { get; set; }
        public DateTime LastEditDate { get; set; }
        public ICollection<Post> Posts { get; set; } = new List<Post>();
    }
}
