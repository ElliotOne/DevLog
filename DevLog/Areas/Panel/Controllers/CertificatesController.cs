using AutoMapper;
using DevLog.Areas.Panel.Extensions;
using DevLog.Areas.Panel.Models;
using DevLog.Areas.Panel.Models.ViewModels;
using DevLog.Core.Domain;
using DevLog.Models.Constants;
using DevLog.Persistence;
using DevLog.Services.FileHandler;
using DevLog.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DevLog.Areas.Panel.Controllers
{
    [Area(AreasConstant.Panel)]
    [Authorize(Roles = UserRolesConstant.SuperAdmin + "," + UserRolesConstant.Admin)]
    public class CertificatesController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IFileHandler _fileHandler;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public CertificatesController(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IFileHandler fileHandler,
            IWebHostEnvironment webHostEnvironment)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _fileHandler = fileHandler;
            _webHostEnvironment = webHostEnvironment;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> LoadCertificatesTable([FromBody] DTParameters dtParameters)
        {
            var searchBy = dtParameters.Search?.Value;

            string orderCriteria;
            bool orderAscendingDirection;

            if (dtParameters.Order == null)
            {
                //When Empty search will be order the results by Id ascending
                orderCriteria = nameof(IEntity.Id);
                orderAscendingDirection = true;
            }
            else
            {
                //Default sort on the 1st column
                orderCriteria = dtParameters.Columns![dtParameters.Order[0].Column].Data!;
                orderAscendingDirection =
                    string.Equals(
                        dtParameters.Order[0].Dir.ToString(),
                        nameof(LinqExtensions.Order.Asc),
                        StringComparison.CurrentCultureIgnoreCase);
            }

            var result = _unitOfWork.CertificateRepository.GetAll();

            if (!string.IsNullOrWhiteSpace(searchBy))
            {
                result = result.Where(x =>
                    (x.Title.Contains(searchBy)) ||
                    (x.CreateDate.ToString("F").Contains(searchBy)) ||
                    (x.LastEditDate.ToString("F").Contains(searchBy))
                );
            }

            result = orderAscendingDirection ?
                result.OrderByDynamic(orderCriteria, LinqExtensions.Order.Asc) :
                result.AsQueryable().OrderByDynamic(orderCriteria, LinqExtensions.Order.Desc);

            var filteredResultsCount = result.Count();
            var totalResultsCount = await _unitOfWork.ContactRepository.Count();

            var resultList = result
                .Skip(dtParameters.Start)
                .Take(dtParameters.Length)
                .ToList();

            return new JsonResult(new DatatableJsonResultModel()
            {
                Draw = dtParameters.Draw,
                RecordsTotal = totalResultsCount,
                RecordsFiltered = filteredResultsCount,
                Data =
                    _mapper.Map<IEnumerable<Certificate>, IEnumerable<CertificateIndexViewModel>>(resultList)
            });
        }

        public async Task<IActionResult> Details(int id)
        {
            var certificate = await _unitOfWork.CertificateRepository.GetById(id);

            if (certificate == null)
            {
                return NotFound();
            }

            return View(_mapper.Map<Certificate, CertificateFormViewModel>(certificate));
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CertificateFormViewModel certificateFormViewModel)
        {
            if (!ModelState.IsValid)
            {
                return View(certificateFormViewModel);
            }

            var certificate = _mapper.Map<CertificateFormViewModel, Certificate>(certificateFormViewModel);

            var files = HttpContext.Request.Form.Files;

            if (files.Any())
            {
                certificate.ImageVirtualPath =
                    _fileHandler.GetRelativePath(
                        files[0].FileName,
                        GuidUtility.NewGuidSafeString(),
                        FileType.Certificate);

                await _fileHandler.Upload(
                    files[0],
                    certificate.ImageVirtualPath);
            }

            _unitOfWork.CertificateRepository.Insert(certificate);
            await _unitOfWork.Complete();

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int id)
        {
            var certificate = await _unitOfWork.CertificateRepository.GetById(id);

            if (certificate == null)
            {
                return NotFound();
            }

            return View(_mapper.Map<Certificate, CertificateFormViewModel>(certificate));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, CertificateFormViewModel certificateFormViewModel)
        {
            if (id != certificateFormViewModel.Id)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                return View(certificateFormViewModel);
            }

            var certificate = _mapper.Map<CertificateFormViewModel, Certificate>(certificateFormViewModel);

            var files = HttpContext.Request.Form.Files;

            if (files.Any())
            {
                //Delete the old file
                if (!string.IsNullOrWhiteSpace(certificate.ImageVirtualPath))
                {
                    _fileHandler.Delete(certificate.ImageVirtualPath);
                }

                //Upload the new file
                certificate.ImageVirtualPath =
                    _fileHandler.GetRelativePath(
                        files[0].FileName,
                        GuidUtility.NewGuidSafeString(),
                        FileType.Certificate);

                await _fileHandler.Upload(
                    files[0],
                    certificate.ImageVirtualPath);
            }

            _unitOfWork.CertificateRepository.Update(certificate);
            await _unitOfWork.Complete();

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int id)
        {
            var certificate = await _unitOfWork.CertificateRepository.GetById(id);

            if (certificate == null)
            {
                return NotFound();
            }

            return View(_mapper.Map<Certificate, CertificateFormViewModel>(certificate));
        }

        [HttpPost, ActionName(nameof(Delete))]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id, string? imageVirtualPath)
        {
            if (imageVirtualPath == null)
            {
                return BadRequest();
            }

            _fileHandler.Delete(imageVirtualPath);

            await _unitOfWork.CertificateRepository.Delete(id);
            await _unitOfWork.Complete();

            return RedirectToAction(nameof(Index));
        }
    }
}
