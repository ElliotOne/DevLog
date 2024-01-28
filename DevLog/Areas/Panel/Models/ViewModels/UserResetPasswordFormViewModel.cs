using DevLog.Models.Constants;
using System.ComponentModel.DataAnnotations;

namespace DevLog.Areas.Panel.Models.ViewModels
{
    public class UserResetPasswordFormViewModel
    {
        [Required(ErrorMessage = ValidationErrorMessagesConstant.RequiredMsg)]
        [MaxLength(256, ErrorMessage = ValidationErrorMessagesConstant.MaxLengthMsg)]
        [EmailAddress(ErrorMessage = ValidationErrorMessagesConstant.RegexMsg)]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = ValidationErrorMessagesConstant.RequiredMsg)]
        [StringLength(30, MinimumLength = 6, ErrorMessage = ValidationErrorMessagesConstant.StringLengthMsg)]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        [Display(Name = "Confirm password")]
        [Required(ErrorMessage = ValidationErrorMessagesConstant.RequiredMsg)]
        [Compare(nameof(Password), ErrorMessage = ValidationErrorMessagesConstant.CompareMsg)]
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; } = string.Empty;

        public string Token { get; set; } = string.Empty;
    }
}