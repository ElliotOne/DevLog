using DevLog.Core.Domain;
using DevLog.Models.Constants;
using System.ComponentModel.DataAnnotations;

namespace DevLog.Areas.Panel.Models.ViewModels
{
    public class PostCommentFormViewModel : IEntity
    {
        public int Id { get; set; }

        [Display(Name = "Sender")]
        [Required(ErrorMessage = ValidationErrorMessagesConstant.RequiredMsg)]
        public string UserFullName { get; set; } = string.Empty;

        [Display(Name = "Post")]
        public string PostTitle { get; set; } = string.Empty;

        [Display(Name = "Comment")]
        [Required(ErrorMessage = ValidationErrorMessagesConstant.RequiredMsg)]
        [MaxLength(1024, ErrorMessage = ValidationErrorMessagesConstant.MaxLengthMsg)]
        public string Body { get; set; } = string.Empty;

        [Display(Name = "Email")]
        [Required(ErrorMessage = ValidationErrorMessagesConstant.RequiredMsg)]
        [MaxLength(256, ErrorMessage = ValidationErrorMessagesConstant.MaxLengthMsg)]
        public string Email { get; set; } = string.Empty;

        public PostCommentStatus Status { get; set; }

        [Display(Name = "Sender's IP")]
        public string Ip { get; set; } = string.Empty;

        [Display(Name = "Edited")]
        public bool IsEdited { get; set; }

        [Display(Name = "Created On")]
        public DateTime CreateDate { get; set; }

        [Display(Name = "Last Modified On")]
        public DateTime LastEditDate { get; set; }

        public int? ParentId { get; set; }

        public int UserId { get; set; }

        public int PostId { get; set; }

        public int PostUserId { get; set; }
    }
}