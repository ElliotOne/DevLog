using DevLog.Persistence;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DevLog.ViewComponents
{
    public class ProgressBarsViewComponent : ViewComponent
    {
        private readonly IUnitOfWork _unitOfWork;

        public ProgressBarsViewComponent(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var progressBars = await _unitOfWork.ProgressBarRepository
                .GetAll()
                .OrderBy(x => x.SortIndex)
                .ToListAsync();

            return View(progressBars);
        }
    }
}
