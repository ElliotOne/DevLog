using DevLog.Core.Domain;
using DevLog.Models.Constants;
using System.ComponentModel.DataAnnotations;

namespace DevLog.Areas.Panel.Models.ViewModels
{
    public class ProgressBarFormViewModel : IEntity
    {
        public int Id { get; set; }

        [Required(ErrorMessage = ValidationErrorMessagesConstant.RequiredMsg)]
        [MaxLength(256, ErrorMessage = ValidationErrorMessagesConstant.MaxLengthMsg)]
        public string Topic { get; set; } = string.Empty;

        [Required(ErrorMessage = ValidationErrorMessagesConstant.RequiredMsg)]
        [Range(0, 100, ErrorMessage = ValidationErrorMessagesConstant.RangeMsg)]
        public int Percentage { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = ValidationErrorMessagesConstant.RangeMsg)]
        public int SortIndex { get; set; }
    }
}
