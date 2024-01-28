using DevLog.Models.Constants;
using System.ComponentModel.DataAnnotations;

namespace DevLog.Areas.Panel.Models.ViewModels
{
    public class UserCreateFormViewModel
    {
        [Display(Name = "Username")]
        [Required(ErrorMessage = ValidationErrorMessagesConstant.RequiredMsg)]
        [StringLength(30, MinimumLength = 6, ErrorMessage = ValidationErrorMessagesConstant.StringLengthMsg)]
        public string UserName { get; set; } = string.Empty;

        [Required(ErrorMessage = ValidationErrorMessagesConstant.RequiredMsg)]
        [StringLength(30, MinimumLength = 6, ErrorMessage = ValidationErrorMessagesConstant.StringLengthMsg)]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        [Display(Name = "Confirm password")]
        [Required(ErrorMessage = ValidationErrorMessagesConstant.RequiredMsg)]
        [Compare(nameof(Password), ErrorMessage = ValidationErrorMessagesConstant.CompareMsg)]
        [DataType(DataType.Password)]
        public string PasswordConfirm { get; set; } = string.Empty;

        [Display(Name = "Firstname")]
        [Required(ErrorMessage = ValidationErrorMessagesConstant.RequiredMsg)]
        [StringLength(256, MinimumLength = 3, ErrorMessage = ValidationErrorMessagesConstant.StringLengthMsg)]
        public string FirstName { get; set; } = string.Empty;

        [Display(Name = "Lastname")]
        [Required(ErrorMessage = ValidationErrorMessagesConstant.RequiredMsg)]
        [StringLength(256, MinimumLength = 3, ErrorMessage = ValidationErrorMessagesConstant.StringLengthMsg)]
        public string LastName { get; set; } = string.Empty;

        [Required(ErrorMessage = ValidationErrorMessagesConstant.RequiredMsg)]
        [MaxLength(256, ErrorMessage = ValidationErrorMessagesConstant.MaxLengthMsg)]
        [EmailAddress(ErrorMessage = ValidationErrorMessagesConstant.RegexMsg)]
        public string Email { get; set; } = string.Empty;

        [MaxLength(1000, ErrorMessage = ValidationErrorMessagesConstant.MaxLengthMsg)]
        public string? Biography { get; set; }

        [Display(Name = "Active")]
        public bool IsActive { get; set; }

        [Display(Name = "Profile image")]
        public IFormFile? File { get; set; }

        public UserRole Role { get; set; }
    }
}