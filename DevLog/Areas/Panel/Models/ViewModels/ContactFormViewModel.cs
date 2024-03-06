using DevLog.Core.Domain;
using DevLog.Models.Constants;
using System.ComponentModel.DataAnnotations;

namespace DevLog.Areas.Panel.Models.ViewModels
{
    public class ContactFormViewModel : IEntity
    {
        public int Id { get; set; }

        [Display(Name = "Sender")]
        [Required(ErrorMessage = ValidationErrorMessagesConstant.RequiredMsg)]
        [MaxLength(256, ErrorMessage = ValidationErrorMessagesConstant.MaxLengthMsg)]
        public string UserFullName { get; set; } = string.Empty;

        [Display(Name = "Subject")]
        [Required(ErrorMessage = ValidationErrorMessagesConstant.RequiredMsg)]
        [MaxLength(256, ErrorMessage = ValidationErrorMessagesConstant.MaxLengthMsg)]
        public string Subject { get; set; } = string.Empty;

        [Display(Name = "Email/Phone number")]
        [Required(ErrorMessage = ValidationErrorMessagesConstant.RequiredMsg)]
        [MaxLength(256, ErrorMessage = ValidationErrorMessagesConstant.MaxLengthMsg)]
        public string EmailOrPhoneNumber { get; set; } = string.Empty;

        [Display(Name = "Description")]
        [Required(ErrorMessage = ValidationErrorMessagesConstant.RequiredMsg)]
        [MaxLength(1024, ErrorMessage = ValidationErrorMessagesConstant.MaxLengthMsg)]
        public string Body { get; set; } = string.Empty;

        [Display(Name = "Sender's IP")]
        public string Ip { get; set; } = string.Empty;

        [Display(Name = "Created On")]
        public DateTime CreateDate { get; set; }

        [Display(Name = "Last Modified On")]
        public DateTime LastEditDate { get; set; }
    }
}
