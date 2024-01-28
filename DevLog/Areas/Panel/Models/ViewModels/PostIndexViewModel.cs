using DevLog.Core.Domain;
using System.ComponentModel.DataAnnotations;

namespace DevLog.Areas.Panel.Models.ViewModels
{
    public class PostIndexViewModel : IEntity
    {
        public int Id { get; set; }

        public string Title { get; set; } = string.Empty;

        [Display(Name = "Category")]
        public string? PostCategoryTitle { get; set; }

        [Display(Name = "Tags")]
        public string? Tags { get; set; }

        [Display(Name = "Writer")]
        public string UserFullName { get; set; } = string.Empty;

        [Display(Name = "Comments enabled")]
        public bool IsCommentsOn { get; set; }

        [Display(Name = "Created On")]
        public DateTime CreateDate { get; set; }

        [Display(Name = "Last Modified On")]
        public DateTime LastEditDate { get; set; }
    }
}
