using AutoMapper;
using DevLog.Areas.Panel.Extensions;
using DevLog.Areas.Panel.Models;
using DevLog.Areas.Panel.Models.ViewModels;
using DevLog.Core.Domain;
using DevLog.Models.Constants;
using DevLog.Persistence;
using DevLog.Services.EmailSender;
using DevLog.Services.FileHandler;
using DevLog.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DevLog.Areas.Panel.Controllers
{
    [Area(AreasConstant.Panel)]
    [Authorize(Roles = UserRolesConstant.SuperAdmin + "," + UserRolesConstant.Admin + "," + UserRolesConstant.Writer)]
    public class UsersController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IEmailSender _emailSender;
        private readonly IFileHandler _fileHandler;

        public UsersController(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IEmailSender emailSender,
            IFileHandler fileHandler)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _emailSender = emailSender;
            _fileHandler = fileHandler;
        }

        [Authorize(Roles = UserRolesConstant.SuperAdmin)]
        public IActionResult Index()
        {
            return View();
        }

        [Authorize(Roles = UserRolesConstant.SuperAdmin)]
        [HttpPost]
        public async Task<IActionResult> LoadUsersTable([FromBody] DTParameters dtParameters)
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

            var result = _unitOfWork.UserRepository.GetAll();

            if (!string.IsNullOrWhiteSpace(searchBy))
            {
                result = result.Where(x =>
                    (x.FirstName.Contains(searchBy)) ||
                    (x.LastName.Contains(searchBy)) ||
                    (x.Email.Contains(searchBy)) ||
                    (x.CreateDate.ToString("F").Contains(searchBy)) ||
                    (x.LastEditDate.ToString("F").Contains(searchBy))
                );
            }

            if (string.Equals(orderCriteria,
                nameof(UserIndexViewModel.UserFullName), StringComparison.InvariantCultureIgnoreCase))
            {
                result = orderAscendingDirection
                    ? result.OrderBy(x => x.FirstName + " " + x.LastName)
                    : result.OrderByDescending(x => x.FirstName + " " + x.LastName);
            }
            else
            {
                result = orderAscendingDirection ?
                    result.OrderByDynamic(orderCriteria, LinqExtensions.Order.Asc) :
                    result.AsQueryable().OrderByDynamic(orderCriteria, LinqExtensions.Order.Desc);
            }

            var filteredResultsCount = result.Count();
            var totalResultsCount = await _unitOfWork.UserRepository.Count();

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
                    .Map<IEnumerable<User>, IEnumerable<UserIndexViewModel>>(resultList)
            });
        }

        public async Task<IActionResult> Details(int id)
        {
            var user = await _unitOfWork.UserRepository.GetById(id);

            if (user == null)
            {
                return NotFound();
            }

            var currentUser = (await _unitOfWork.UserRepository.GetByClaimsPrincipal(HttpContext.User))!;

            if (!await _unitOfWork.UserRepository
                .IsUserAllowedForOperation(currentUser, user.Id, UserRolesConstant.SuperAdmin))
            {
                return Unauthorized();
            }

            return View(_mapper.Map<User, UserFormViewModel>(user));
        }

        [Authorize(Roles = UserRolesConstant.SuperAdmin)]
        public IActionResult Create()
        {
            return View();
        }

        [Authorize(Roles = UserRolesConstant.SuperAdmin)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(UserCreateFormViewModel userCreateFormViewModel)
        {
            if (userCreateFormViewModel.Role == UserRole.SuperAdmin)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                return View(userCreateFormViewModel);
            }

            var user = _mapper.Map<UserCreateFormViewModel, User>(userCreateFormViewModel);

            var files = HttpContext.Request.Form.Files;

            if (files.Any())
            {
                user.ImageVirtualPath =
                    _fileHandler.GetRelativePath(
                        files[0].FileName,
                        GuidUtility.NewGuidSafeString(),
                        FileType.Profile);

                await _fileHandler.Upload(
                    files[0],
                    user.ImageVirtualPath);
            }

            var addToIdentityResult = await _unitOfWork.UserRepository.Insert(user, userCreateFormViewModel.Password);

            if (!addToIdentityResult.Succeeded)
            {
                if (addToIdentityResult.Errors.Any(c => c.Code == "DuplicateUserName"))
                {
                    ModelState.AddModelError(nameof(userCreateFormViewModel.UserName),
                        MessagesConstant.IdentityDuplicateUserName);
                }

                if (addToIdentityResult.Errors.Any(c => c.Code.Contains("InvalidUserName")))
                {
                    ModelState.AddModelError(nameof(userCreateFormViewModel.UserName), MessagesConstant.IdentityInvalidUserName);
                }

                if (addToIdentityResult.Errors.Any(c => c.Code.Contains("Email")))
                {
                    ModelState.AddModelError(nameof(userCreateFormViewModel.Email), MessagesConstant.IdentityEmail);
                }

                if (addToIdentityResult.Errors.Any(c => c.Code.Contains("Password")))
                {
                    ModelState.AddModelError(nameof(userCreateFormViewModel.Password), MessagesConstant.IdentityPassword);
                }

                return View(userCreateFormViewModel);
            }

            var addToRoleResult =
                await _unitOfWork.UserRepository.AddToRole(user, userCreateFormViewModel.Role.ToString());

            if (!addToRoleResult.Succeeded)
            {
                ViewData["Error"] = MessagesConstant.IdentityAddToRoleResult;
                return View(userCreateFormViewModel);
            }

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int id)
        {
            var user = await _unitOfWork.UserRepository.GetById(id);

            if (user == null)
            {
                return NotFound();
            }

            var currentUser = (await _unitOfWork.UserRepository.GetByClaimsPrincipal(HttpContext.User))!;

            if (!await _unitOfWork.UserRepository
                .IsUserAllowedForOperation(currentUser, user.Id, UserRolesConstant.SuperAdmin))
            {
                return Unauthorized();
            }

            return View(_mapper.Map<User, UserFormViewModel>(user));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, UserFormViewModel userFormViewModel)
        {
            if (id != userFormViewModel.Id)
            {
                return BadRequest();
            }

            var user = await _unitOfWork.UserRepository.GetById(userFormViewModel.Id);

            if (user == null)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                return View(userFormViewModel);
            }

            var currentUser = (await _unitOfWork.UserRepository.GetByClaimsPrincipal(HttpContext.User))!;

            if (!await _unitOfWork.UserRepository
                .IsUserAllowedForOperation(currentUser, user.Id, UserRolesConstant.SuperAdmin))
            {
                return Unauthorized();
            }

            user.UserName = userFormViewModel.UserName;
            user.FirstName = userFormViewModel.FirstName;
            user.LastName = userFormViewModel.LastName;
            user.Email = userFormViewModel.Email;
            user.Biography = userFormViewModel.Biography;

            if (userFormViewModel.Role != UserRole.SuperAdmin)
            {
                user.IsActive = userFormViewModel.IsActive;
            }

            var files = HttpContext.Request.Form.Files;

            if (files.Any())
            {
                //Delete the old file
                if (!string.IsNullOrWhiteSpace(user.ImageVirtualPath))
                {
                    _fileHandler.Delete(user.ImageVirtualPath);
                }

                //Upload the new file
                user.ImageVirtualPath =
                    _fileHandler.GetRelativePath(
                        files[0].FileName,
                        GuidUtility.NewGuidSafeString(),
                        FileType.Profile);

                await _fileHandler.Upload(
                files[0],
                user.ImageVirtualPath);
            }

            var result = await _unitOfWork.UserRepository.Update(user, userFormViewModel.Password);

            if (result.Succeeded)
            {
                await _unitOfWork.UserRepository.SignOut();

                return RedirectToAction(nameof(Index));
            }

            ViewData["Error"] = MessagesConstant.IdentityError;
            return View(userFormViewModel);
        }

        [Authorize(Roles = UserRolesConstant.SuperAdmin)]
        public async Task<JsonResult> ToggleActive(int id)
        {
            var user = await _unitOfWork.UserRepository.GetById(id);

            if (user == null)
            {
                return new JsonResult(BadRequest());
            }

            var isSuperAdmin = await _unitOfWork.UserRepository.IsInRole(user, UserRolesConstant.SuperAdmin);

            if (isSuperAdmin)
            {
                return new JsonResult(BadRequest());
            }

            user.IsActive = !user.IsActive;
            await _unitOfWork.UserRepository.Update(user);

            return new JsonResult(Ok());
        }

        [AllowAnonymous]
        public IActionResult Login(string? returnUrl)
        {
            ViewBag.returnUrl = returnUrl;
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(UserLoginFormViewModel userLoginFormViewModel, string? returnUrl)
        {
            ViewBag.returnUrl = returnUrl;

            if (!ModelState.IsValid)
            {
                return View(userLoginFormViewModel);
            }

            var user = await _unitOfWork.UserRepository.GetByUserName(userLoginFormViewModel.Username);

            if (user == null)
            {
                ModelState.AddModelError(nameof(userLoginFormViewModel.Username),
                    MessagesConstant.IdentityNotRegisteredYet);

                return View(userLoginFormViewModel);
            }

            if (user.IsActive)
            {
                await _unitOfWork.UserRepository.SignOut();

                var result = await _unitOfWork.UserRepository
                    .SignInByPassword(
                        user,
                        userLoginFormViewModel.Password,
                        userLoginFormViewModel.RememberMe,
                        false
                    );

                if (result.Succeeded)
                {
                    ViewBag.returnUrl = returnUrl;
                    return LocalRedirect(returnUrl ?? "/");
                }

                ModelState.AddModelError(nameof(userLoginFormViewModel.Password),
                    MessagesConstant.IdentityError);

                return View(userLoginFormViewModel);
            }

            ViewData["Error"] = MessagesConstant.IdentityAccountDisabled;
            return View(userLoginFormViewModel);
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public async Task<IActionResult> ForgotPassword(UserForgotPasswordFormViewModel userForgotPasswordFormViewModel)
        {
            if (ModelState.IsValid)
            {
                return View(userForgotPasswordFormViewModel);
            }

            var user = await _unitOfWork.UserRepository.GetByEmail(userForgotPasswordFormViewModel.Email);

            if (user == null)
            {
                ViewData["Error"] = MessagesConstant.IdentityNotRegisteredYet;
                return View(userForgotPasswordFormViewModel);
            }

            var code = await _unitOfWork.UserRepository.GeneratePasswordResetToken(user);
            var callbackUrl = Url.ResetPasswordCallbackLink(user.Id, code, Request.Scheme);
            var resetLink = $"<a href=\"{callbackUrl}\">link</a>";
            var isEmailSent = _emailSender
                .SendEmail(userForgotPasswordFormViewModel.Email,
                    MessagesConstant.VerificationEmailSubject,
                    string.Format(MessagesConstant.VerificationEmailText, resetLink));

            if (isEmailSent)
            {
                return RedirectToAction(nameof(ForgotPasswordConfirmation));
            }

            ViewData["Error"] = MessagesConstant.EmailFailedOnSending;
            return View(userForgotPasswordFormViewModel);
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult ForgotPasswordConfirmation()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _unitOfWork.UserRepository.SignOut();
            return RedirectToAction(nameof(HomeController.Index), "Home");
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult ResetPassword(string token)
        {
            return View(new UserResetPasswordFormViewModel() { Token = token });
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(UserResetPasswordFormViewModel userResetPasswordFormViewModel)
        {
            if (!ModelState.IsValid)
            {
                return View(userResetPasswordFormViewModel);
            }

            var user = await _unitOfWork.UserRepository.GetByEmail(userResetPasswordFormViewModel.Email);

            if (user == null)
            {
                // Don't reveal that the user does not exist
                return RedirectToAction(nameof(ResetPasswordConfirmation));
            }

            var result = await _unitOfWork.UserRepository
                .ResetPassword(user, userResetPasswordFormViewModel.Token, userResetPasswordFormViewModel.Password);

            if (result.Succeeded)
            {
                await _unitOfWork.UserRepository.SignIn(user, false);
                return RedirectToAction(nameof(ResetPasswordConfirmation));
            }

            ViewData["Error"] = MessagesConstant.IdentityUpdatePassword;
            return View(userResetPasswordFormViewModel);
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult ResetPasswordConfirmation()
        {
            return View();
        }
    }
}
