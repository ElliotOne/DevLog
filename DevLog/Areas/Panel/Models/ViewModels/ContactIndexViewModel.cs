using DevLog.Core.Domain;
using System.ComponentModel.DataAnnotations;

namespace DevLog.Areas.Panel.Models.ViewModels
{
    public class ContactIndexViewModel : IEntity
    {
        public int Id { get; set; }

        [Display(Name = "Sender")]
        public string UserFullName { get; set; } = string.Empty;

        [Display(Name = "Subject")]
        public string Subject { get; set; } = string.Empty;

        [Display(Name = "Email/Phone number")]
        public string EmailOrPhoneNumber { get; set; } = string.Empty;

        [Display(Name = "Description")]
        public string Body { get; set; } = string.Empty;

        [Display(Name = "Sender's IP")]
        public string Ip { get; set; } = string.Empty;

        [Display(Name = "Created On")]
        public DateTime CreateDate { get; set; }

        [Display(Name = "Last Modified On")]
        public DateTime LastEditDate { get; set; }
    }
}
