using DevLog.Core.Domain;

namespace DevLog.Models.ViewModels
{
    public class PostCategoryViewModel : IEntity
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public int PostsCount { get; set; }
    }
}
