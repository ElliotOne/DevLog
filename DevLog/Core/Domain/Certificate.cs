namespace DevLog.Core.Domain
{
    public class Certificate : IEntity
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Url { get; set; }
        public string? ImageVirtualPath { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime LastEditDate { get; set; }
    }
}
