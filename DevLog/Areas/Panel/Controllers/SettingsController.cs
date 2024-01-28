using AutoMapper;
using DevLog.Areas.Panel.Models.ViewModels;
using DevLog.Core.Domain;
using DevLog.Models.Constants;
using DevLog.Persistence;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DevLog.Areas.Panel.Controllers
{
    [Area(AreasConstant.Panel)]
    [Authorize(Roles = UserRolesConstant.SuperAdmin)]
    public class SettingsController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public SettingsController(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<IActionResult> Index()
        {
            var setting = await _unitOfWork.SettingRepository.Get();

            if (setting == null)
            {
                return NotFound();
            }

            return View(_mapper.Map<Setting, SettingFormViewModel>(setting));
        }

        [HttpPost, ActionName(nameof(Index))]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> IndexPost(int id, SettingFormViewModel settingFormViewModel)
        {
            if (id != settingFormViewModel.Id)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                return View(settingFormViewModel);
            }

            var setting = _mapper.Map<SettingFormViewModel, Setting>(settingFormViewModel);

            _unitOfWork.SettingRepository.Update(setting);
            await _unitOfWork.Complete();

            return RedirectToAction(nameof(Index));
        }
    }
}
