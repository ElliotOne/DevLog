using AutoMapper;
using DevLog.Areas.Panel.Extensions;
using DevLog.Areas.Panel.Models;
using DevLog.Areas.Panel.Models.ViewModels;
using DevLog.Core.Domain;
using DevLog.Models.Constants;
using DevLog.Persistence;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DevLog.Areas.Panel.Controllers
{
    [Area(AreasConstant.Panel)]
    [Authorize(Roles = UserRolesConstant.SuperAdmin + "," + UserRolesConstant.Admin)]
    public class ContactsController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public ContactsController(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> LoadContactsTable([FromBody] DTParameters dtParameters)
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

            var result = _unitOfWork.ContactRepository.GetAll();

            if (!string.IsNullOrWhiteSpace(searchBy))
            {
                result = result.Where(x =>
                    (x.UserFullName.Contains(searchBy)) ||
                    (x.EmailOrPhoneNumber.Contains(searchBy)) ||
                    (x.Subject.Contains(searchBy)) ||
                    (x.Body.Contains(searchBy)) ||
                    (x.Ip.Contains(searchBy)) ||
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
                Data = _mapper
                    .Map<IEnumerable<Contact>, IEnumerable<ContactIndexViewModel>>(resultList)
            });
        }

        public async Task<IActionResult> Details(int id)
        {
            var contact = await _unitOfWork.ContactRepository.GetById(id);

            if (contact == null)
            {
                return NotFound();
            }

            return View(_mapper.Map<Contact, ContactFormViewModel>(contact));
        }

        public async Task<IActionResult> Edit(int id)
        {
            var contact = await _unitOfWork.ContactRepository.GetById(id);

            if (contact == null)
            {
                return NotFound();
            }

            return View(_mapper.Map<Contact, ContactFormViewModel>(contact));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ContactFormViewModel contactFormViewModel)
        {
            if (id != contactFormViewModel.Id)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                return View(contactFormViewModel);
            }

            var contact = _mapper.Map<ContactFormViewModel, Contact>(contactFormViewModel);

            _unitOfWork.ContactRepository.Update(contact);
            await _unitOfWork.Complete();

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int id)
        {
            var contact = await _unitOfWork.ContactRepository.GetById(id);

            if (contact == null)
            {
                return NotFound();
            }

            return View(_mapper.Map<Contact, ContactFormViewModel>(contact));
        }

        [HttpPost, ActionName(nameof(Delete))]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _unitOfWork.ContactRepository.Delete(id);
            await _unitOfWork.Complete();

            return RedirectToAction(nameof(Index));
        }
    }
}
