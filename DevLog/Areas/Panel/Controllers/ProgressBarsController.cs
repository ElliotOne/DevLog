using AutoMapper;
using DevLog.Areas.Panel.Models.ViewModels;
using DevLog.Core.Domain;
using DevLog.Models.Constants;
using DevLog.Models.Shared.JsonResults;
using DevLog.Persistence;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DevLog.Areas.Panel.Controllers
{
    [Area(AreasConstant.Panel)]
    [Authorize(Roles = UserRolesConstant.SuperAdmin + "," + UserRolesConstant.Admin)]
    public class ProgressBarsController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public ProgressBarsController(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<IActionResult> Index()
        {
            var progressBars =
                await _unitOfWork.ProgressBarRepository.GetAll()
                    .OrderBy(x => x.SortIndex)
                    .ToListAsync();

            var progressBarIndexViewModel = new ProgressBarIndexViewModel()
            {
                ProgressBarFormViewModels =
                    _mapper.Map<IEnumerable<ProgressBar>, IList<ProgressBarFormViewModel>>(progressBars)
            };

            return View(progressBarIndexViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(int id, ProgressBarIndexViewModel progressBarIndexViewModel)
        {
            if (id != progressBarIndexViewModel.Id)
            {
                return BadRequest();
            }

            if (ModelState.IsValid)
            {
                var progressBars =
                    _mapper.Map<IList<ProgressBarFormViewModel>, IEnumerable<ProgressBar>>(progressBarIndexViewModel.ProgressBarFormViewModels);

                _unitOfWork.ProgressBarRepository.UpdateAll(progressBars);
                await _unitOfWork.Complete();

                return RedirectToAction(nameof(Index));
            }

            return View(progressBarIndexViewModel);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProgressBarFormViewModel progressBarFormViewModel)
        {
            if (!ModelState.IsValid)
            {
                return View(progressBarFormViewModel);
            }

            var progressBar =
                _mapper.Map<ProgressBarFormViewModel, ProgressBar>(progressBarFormViewModel);

            await _unitOfWork.ProgressBarRepository.Insert(progressBar);
            await _unitOfWork.Complete();

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<JsonResult> Delete(int id)
        {
            await _unitOfWork.ProgressBarRepository.Delete(id);
            await _unitOfWork.Complete();

            return new JsonResult(new JsonResultModel()
            {
                StatusCode = JsonResultStatusCode.Success,
                RedirectUrl = Url.Action(nameof(Index), "ProgressBars")
            });
        }
    }
}
