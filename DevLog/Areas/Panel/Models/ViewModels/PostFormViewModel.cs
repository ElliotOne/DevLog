using DevLog.Core.Domain;
using DevLog.Models.Constants;
using System.ComponentModel.DataAnnotations;

namespace DevLog.Areas.Panel.Models.ViewModels
{
    public class PostFormViewModel : IEntity
    {
        public int Id { get; set; }

        [Required(ErrorMessage = ValidationErrorMessagesConstant.RequiredMsg)]
        [MaxLength(256, ErrorMessage = ValidationErrorMessagesConstant.MaxLengthMsg)]
        public string Title { get; set; } = string.Empty;

        [Display(Name = "Body")]
        [Required(ErrorMessage = ValidationErrorMessagesConstant.RequiredMsg)]
        public string Body { get; set; } = string.Empty;

        [Display(Name = "Created On")]
        public DateTime CreateDate { get; set; }

        [Display(Name = "Last Modified On")]
        public DateTime LastEditDate { get; set; }

        [Display(Name = "Tags")]
        [MaxLength(1000, ErrorMessage = ValidationErrorMessagesConstant.MaxLengthMsg)]
        public string? Tags { get; set; }

        [Display(Name = "Category")]
        public int? PostCategoryId { get; set; }

        public int UserId { get; set; }

        [Display(Name = "Writer")]
        public string UserFullName { get; set; } = string.Empty;

        [Display(Name = "Comments enabled")]
        public bool IsCommentsOn { get; set; }

        [Display(Name = "Image")]
        public IFormFile? File { get; set; }

        public string? ImageVirtualPath { get; set; }
    }
}
