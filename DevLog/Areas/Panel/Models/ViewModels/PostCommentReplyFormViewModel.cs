using DevLog.Core.Domain;
using DevLog.Models.Constants;
using System.ComponentModel.DataAnnotations;

namespace DevLog.Areas.Panel.Models.ViewModels
{
    public class PostCommentReplyFormViewModel : IEntity
    {
        public int Id { get; set; }

        public int PostId { get; set; }

        public int ParentId { get; set; }


        [Display(Name = "Parent comment")]
        public string ParentBody { get; set; } = string.Empty;


        [Display(Name = "Comment")]
        [Required(ErrorMessage = ValidationErrorMessagesConstant.RequiredMsg)]
        [MaxLength(1000, ErrorMessage = ValidationErrorMessagesConstant.MaxLengthMsg)]
        public string Body { get; set; } = string.Empty;

        public PostCommentStatus Status { get; set; }
    }
}
