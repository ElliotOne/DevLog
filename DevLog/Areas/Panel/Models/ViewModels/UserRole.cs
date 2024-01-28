using System.ComponentModel.DataAnnotations;

namespace DevLog.Areas.Panel.Models.ViewModels
{
    public enum UserRole
    {
        [Display(Order = 0)]
        SuperAdmin,

        [Display(Order = 1)]
        Admin,

        [Display(Order = 2)]
        Writer
    }
}
