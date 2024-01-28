using DevLog.Core.Domain;

namespace DevLog.Areas.Panel.Models.ViewModels
{
    public class ProgressBarIndexViewModel : IEntity
    {
        public int Id { get; set; }
        public IList<ProgressBarFormViewModel> ProgressBarFormViewModels { get; set; }
            = new List<ProgressBarFormViewModel>();
    }
}
