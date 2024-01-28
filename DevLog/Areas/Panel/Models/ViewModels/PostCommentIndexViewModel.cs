using DevLog.Core.Domain;
using System.ComponentModel.DataAnnotations;

namespace DevLog.Areas.Panel.Models.ViewModels
{
    public class PostCommentIndexViewModel : IEntity
    {
        public int Id { get; set; }

        [Display(Name = "Sender")]
        public string UserFullName { get; set; } = string.Empty;

        [Display(Name = "Post")]
        public string PostTitle { get; set; } = string.Empty;

        [Display(Name = "Comment")]
        public string Body { get; set; } = string.Empty;

        [Display(Name = "Email")]
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
    }
}
