namespace DevLog.Models.Parameters
{
    public class PostSearchParameters
    {
        public string? SearchString { get; set; }
        public int? PostCategoryId { get; set; }
        public string? PostTag { get; set; }
        public PostSortFilterType PostSortFilterType { get; set; }
    }
}
