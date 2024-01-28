using DevLog.Models.Constants;
using System.ComponentModel.DataAnnotations;

namespace DevLog.Models.ViewModels
{
    public class PostCommentFormViewModel
    {
        public int? ParentId { get; set; }

        public int PostId { get; set; }

        [Display(Name = "Full name")]
        [Required(ErrorMessage = ValidationErrorMessagesConstant.RequiredMsg)]
        [MaxLength(256, ErrorMessage = ValidationErrorMessagesConstant.MaxLengthMsg)]
        public string UserFullName { get; set; } = string.Empty;

        [Display(Name = "Comment")]
        [Required(ErrorMessage = ValidationErrorMessagesConstant.RequiredMsg)]
        [MaxLength(1000, ErrorMessage = ValidationErrorMessagesConstant.MaxLengthMsg)]
        public string Body { get; set; } = string.Empty;

        [Display(Name = "Email")]
        [Required(ErrorMessage = ValidationErrorMessagesConstant.RequiredMsg)]
        [MaxLength(256, ErrorMessage = ValidationErrorMessagesConstant.MaxLengthMsg)]
        [EmailAddress(ErrorMessage = ValidationErrorMessagesConstant.RegexMsg)]
        public string Email { get; set; } = string.Empty;
    }
}
