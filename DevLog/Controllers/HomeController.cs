using AutoMapper;
using DevLog.Core.Domain;
using DevLog.Models.Constants;
using DevLog.Models.Shared.JsonResults;
using DevLog.Models.ViewModels;
using DevLog.Persistence;
using Microsoft.AspNetCore.Mvc;
using SmartBreadcrumbs.Attributes;

namespace DevLog.Controllers
{
    public class HomeController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public HomeController(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IHttpContextAccessor httpContextAccessor)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
        }

        [DefaultBreadcrumb("Home")]
        public IActionResult Index()
        {
            return View();
        }

        [Breadcrumb("Contact Me")]
        public IActionResult Contact()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Contact(ContactFormViewModel contactFormViewModel)
        {
            if (!ModelState.IsValid)
            {
                return new JsonResult(new JsonResultModel()
                {
                    StatusCode = JsonResultStatusCode.ModelStateIsNotValid,
                    Message = MessagesConstant.ContactFailedToSend
                });
            }

            var contact = _mapper.Map<ContactFormViewModel, Contact>(contactFormViewModel);

            if (_httpContextAccessor.HttpContext?.Connection.RemoteIpAddress != null)
            {
                contact.Ip = _httpContextAccessor.HttpContext.Connection.RemoteIpAddress.ToString();
            }

            _unitOfWork.ContactRepository.Insert(contact);
            await _unitOfWork.Complete();

            return new JsonResult(new JsonResultModel()
            {
                StatusCode = JsonResultStatusCode.Success,
                Message = MessagesConstant.ContactSentSuccessfully
            });
        }

        [Breadcrumb("About Me")]
        public IActionResult About()
        {
            return View();
        }

        [Breadcrumb("Donation")]
        public async Task<IActionResult> Donate()
        {
            var settings = await _unitOfWork.SettingRepository.Get();

            return View(_mapper.Map<Setting, DonateViewModel>(settings!));
        }
    }
}