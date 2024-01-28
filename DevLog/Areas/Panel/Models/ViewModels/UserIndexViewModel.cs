using System.ComponentModel.DataAnnotations;

namespace DevLog.Areas.Panel.Models.ViewModels
{
    public class UserIndexViewModel
    {
        public string Id { get; set; } = string.Empty;

        [Display(Name = "Username")]
        public string UserName { get; set; } = string.Empty;

        [Display(Name = "Full name")]
        public string UserFullName { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        [Display(Name = "Active")]
        public bool IsActive { get; set; }

        [Display(Name = "Created On")]
        public DateTime CreateDate { get; set; }

        [Display(Name = "Last Modified On")]
        public DateTime LastEditDate { get; set; }
    }
}
