using System.ComponentModel.DataAnnotations;

namespace DevLog.Areas.Panel.Models.ViewModels
{
    public enum PostCommentStatus
    {
        [Display(Name = "Unclear")]
        Unclear = 1000,

        [Display(Name = "Rejected")]
        Rejected = 2000,

        [Display(Name = "Accepted")]
        Accepted = 3000
    }
}
