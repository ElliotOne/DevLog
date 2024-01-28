using DevLog.Core.Domain;
using DevLog.Models.Constants;
using System.ComponentModel.DataAnnotations;

namespace DevLog.Models.ViewModels
{
    public class ContactFormViewModel : IEntity
    {
        public int Id { get; set; }

        [Display(Name = "Full name")]
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
        [MaxLength(1000, ErrorMessage = ValidationErrorMessagesConstant.MaxLengthMsg)]
        public string Body { get; set; } = string.Empty;
    }
}
