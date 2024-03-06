using DevLog.Core.Domain;
using DevLog.Models.Constants;
using System.ComponentModel.DataAnnotations;

namespace DevLog.Areas.Panel.Models.ViewModels
{
    public class CertificateFormViewModel : IEntity
    {
        public int Id { get; set; }

        [Required(ErrorMessage = ValidationErrorMessagesConstant.RequiredMsg)]
        [MaxLength(256, ErrorMessage = ValidationErrorMessagesConstant.MaxLengthMsg)]
        public string Title { get; set; } = string.Empty;

        [Display(Name = "Link")]
        [MaxLength(1024, ErrorMessage = ValidationErrorMessagesConstant.MaxLengthMsg)]
        public string? Url { get; set; }

        [Display(Name = "Created On")]
        public DateTime CreateDate { get; set; }

        [Display(Name = "Last Modified On")]
        public DateTime LastEditDate { get; set; }

        [Display(Name = "Image")]
        public IFormFile? File { get; set; }

        public string? ImageVirtualPath { get; set; }
    }
}
