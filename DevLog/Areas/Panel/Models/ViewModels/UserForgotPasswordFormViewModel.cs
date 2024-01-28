using DevLog.Models.Constants;
using System.ComponentModel.DataAnnotations;

namespace DevLog.Areas.Panel.Models.ViewModels
{
    public class UserForgotPasswordFormViewModel
    {
        [Required(ErrorMessage = ValidationErrorMessagesConstant.RequiredMsg)]
        [MaxLength(256, ErrorMessage = ValidationErrorMessagesConstant.MaxLengthMsg)]
        [EmailAddress(ErrorMessage = ValidationErrorMessagesConstant.RegexMsg)]
        public string Email { get; set; } = string.Empty;
    }
}
