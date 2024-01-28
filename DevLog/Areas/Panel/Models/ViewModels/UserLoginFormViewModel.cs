using DevLog.Models.Constants;
using System.ComponentModel.DataAnnotations;

namespace DevLog.Areas.Panel.Models.ViewModels
{
    public class UserLoginFormViewModel
    {
        [Display(Name = "Username")]
        [Required(ErrorMessage = ValidationErrorMessagesConstant.RequiredMsg)]
        [StringLength(30, MinimumLength = 6, ErrorMessage = ValidationErrorMessagesConstant.StringLengthMsg)]
        public string Username { get; set; } = string.Empty;

        [Required(ErrorMessage = ValidationErrorMessagesConstant.RequiredMsg)]
        [StringLength(30, MinimumLength = 6, ErrorMessage = ValidationErrorMessagesConstant.StringLengthMsg)]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        [Display(Name = "Remember me")]
        public bool RememberMe { get; set; }
    }
}
