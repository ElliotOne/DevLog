using System.Net.Http;
using System.Security.Claims;
using System.Text;
using AutoMapper;
using DevLog.Areas.Panel.Controllers;
using DevLog.Areas.Panel.Models;
using DevLog.Areas.Panel.Models.ViewModels;
using DevLog.Core.Domain;
using DevLog.Models.Constants;
using DevLog.Persistence;
using DevLog.Services.EmailSender;
using DevLog.Services.FileHandler;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.Extensions.Primitives;
using Moq;

namespace DevLog.UnitTests.Areas.Panel.Controllers
{
    [TestFixture]
    public class UsersControllerTests
    {
        private Mock<IUnitOfWork> _unitOfWorkMock = null!;
        private Mock<IMapper> _mapperMock = null!;
        private Mock<IEmailSender> _emailSenderMock = null!;
        private Mock<IFileHandler> _fileHandlerMock;
        private UsersController _controller = null!;
        private ClaimsPrincipal _userPrincipal;

        [SetUp]
        public void Setup()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _mapperMock = new Mock<IMapper>();
            _emailSenderMock = new Mock<IEmailSender>();
            _fileHandlerMock = new Mock<IFileHandler>();

            _userPrincipal = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, "1")
            }));

            // Create DefaultHttpContext instance
            var httpContext = new DefaultHttpContext
            {
                User = _userPrincipal
            };

            // Create an empty FormCollection (or populate if needed)
            var formCollection = new FormCollection(
                new Dictionary<string, StringValues>(),
                new FormFileCollection()
            );

            // Set antiforgery validation feature FIRST
            httpContext.Features.Set<IAntiforgeryValidationFeature>(new FakeAntiforgeryValidationFeature());

            // Set the form feature so Request.Form is not null
            httpContext.Features.Set<IFormFeature>(new FormFeature(formCollection));

            // Assign HttpContext to ControllerContext
            _controller = new UsersController(
                _unitOfWorkMock.Object,
                _mapperMock.Object,
                _emailSenderMock.Object,
                _fileHandlerMock.Object
            )
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = httpContext
                }
            };
        }

        [Test]
        public void Index_ReturnsView()
        {
            // Act
            var result = _controller.Index();

            // Assert
            Assert.That(result, Is.InstanceOf<ViewResult>());
        }

        [Test]
        public async Task LoadUsersTable_NoOrder_ReturnsJsonResult()
        {
            // Arrange
            var dtParams = new DTParameters()
            {
                Draw = 1,
                Start = 0,
                Length = 10,
                Order = null,
                Search = new DTSearch { Value = "" },
                Columns = new DTColumn[] { new DTColumn { Data = "Id" } }
            };

            var users = new List<User>
            {
                new User { Id = 1, FirstName = "A", LastName = "B", Email = "a@b.com", CreateDate = DateTime.UtcNow, LastEditDate = DateTime.UtcNow },
                new User { Id = 2, FirstName = "C", LastName = "D", Email = "c@d.com", CreateDate = DateTime.UtcNow, LastEditDate = DateTime.UtcNow }
            }.AsQueryable();

            _unitOfWorkMock.Setup(u => u.UserRepository.GetAll()).Returns(users);
            _unitOfWorkMock.Setup(u => u.UserRepository.Count()).ReturnsAsync(users.Count);

            _mapperMock.Setup(m => m.Map<IEnumerable<User>, IEnumerable<UserIndexViewModel>>(It.IsAny<IEnumerable<User>>()))
                .Returns(new List<UserIndexViewModel>());

            // Act
            var result = await _controller.LoadUsersTable(dtParams);

            // Assert
            Assert.That(result, Is.InstanceOf<JsonResult>());

            var json = result as JsonResult;
            Assert.That(json!.Value, Is.Not.Null);

            dynamic data = json.Value!;
            Assert.That(data.Draw, Is.EqualTo(dtParams.Draw));
            Assert.That(data.RecordsTotal, Is.EqualTo(users.Count()));
            Assert.That(data.RecordsFiltered, Is.EqualTo(users.Count()));
        }

        [Test]
        public async Task LoadUsersTable_SearchFiltersUsers()
        {
            // Arrange
            var searchValue = "C";
            var users = new List<User>
            {
                new User { Id = 1, FirstName = "A", LastName = "B", Email = "a@b.com", CreateDate = DateTime.UtcNow, LastEditDate = DateTime.UtcNow },
                new User { Id = 2, FirstName = "C", LastName = "D", Email = "c@d.com", CreateDate = DateTime.UtcNow, LastEditDate = DateTime.UtcNow }
            }.AsQueryable();

            _unitOfWorkMock.Setup(u => u.UserRepository.GetAll()).Returns(users);
            _unitOfWorkMock.Setup(u => u.UserRepository.Count()).ReturnsAsync(users.Count);

            var dtParams = new DTParameters()
            {
                Draw = 1,
                Start = 0,
                Length = 10,
                Order = new DTOrder[] { new DTOrder { Column = 0, Dir = DTOrderDir.ASC } },
                Search = new DTSearch { Value = searchValue },
                Columns = new DTColumn[] { new DTColumn { Data = "FirstName" } }
            };

            _mapperMock.Setup(m => m.Map<IEnumerable<User>, IEnumerable<UserIndexViewModel>>(It.IsAny<IEnumerable<User>>()))
                .Returns(new List<UserIndexViewModel>());

            // Act
            var result = await _controller.LoadUsersTable(dtParams);

            // Assert
            var json = result as JsonResult;
            Assert.That(json, Is.Not.Null);

            dynamic data = json!.Value!;
            Assert.That(data.RecordsFiltered, Is.EqualTo(1));
        }

        [Test]
        public async Task Details_UserNotFound_ReturnsNotFound()
        {
            // Arrange
            _unitOfWorkMock.Setup(u => u.UserRepository.GetById(It.IsAny<int>())).ReturnsAsync((User?)null);

            // Act
            var result = await _controller.Details(123);

            // Assert
            Assert.That(result, Is.TypeOf<NotFoundResult>());
        }

        [Test]
        public async Task Details_UserNotAllowed_ReturnsUnauthorized()
        {
            // Arrange
            var user = new User { Id = 2 };
            var currentUser = new User { Id = 1 };

            _unitOfWorkMock.Setup(u => u.UserRepository.GetById(It.IsAny<int>())).ReturnsAsync(user);
            _unitOfWorkMock.Setup(u => u.UserRepository.GetByClaimsPrincipal(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(currentUser);
            _unitOfWorkMock.Setup(u => u.UserRepository.IsUserAllowedForOperation(currentUser, user.Id, UserRolesConstant.SuperAdmin))
                .ReturnsAsync(false);

            // Act
            var result = await _controller.Details(2);

            // Assert
            Assert.That(result, Is.TypeOf<UnauthorizedResult>());
        }

        [Test]
        public async Task Details_UserAllowed_ReturnsViewWithMappedModel()
        {
            // Arrange
            var user = new User { Id = 2 };
            var currentUser = new User { Id = 1 };
            var mappedVm = new UserFormViewModel { Id = 2 };

            _unitOfWorkMock.Setup(u => u.UserRepository.GetById(It.IsAny<int>())).ReturnsAsync(user);
            _unitOfWorkMock.Setup(u => u.UserRepository.GetByClaimsPrincipal(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(currentUser);
            _unitOfWorkMock.Setup(u => u.UserRepository.IsUserAllowedForOperation(currentUser, user.Id, UserRolesConstant.SuperAdmin))
                .ReturnsAsync(true);
            _mapperMock.Setup(m => m.Map<User, UserFormViewModel>(user)).Returns(mappedVm);

            // Act
            var result = await _controller.Details(2);

            // Assert
            var viewResult = result as ViewResult;
            Assert.That(viewResult, Is.Not.Null);
            Assert.That(viewResult!.Model, Is.EqualTo(mappedVm));
        }

        [Test]
        public void Create_Get_ReturnsView()
        {
            // Act
            var result = _controller.Create();

            // Assert
            Assert.That(result, Is.TypeOf<ViewResult>());
        }

        [Test]
        public async Task Create_Post_SuperAdminRole_ReturnsBadRequest()
        {
            // Arrange
            var model = new UserCreateFormViewModel { Role = UserRole.SuperAdmin };

            // Act
            var result = await _controller.Create(model);

            // Assert
            Assert.That(result, Is.TypeOf<BadRequestResult>());
        }

        [Test]
        public async Task Create_Post_InvalidModel_ReturnsViewWithModel()
        {
            // Arrange
            _controller.ModelState.AddModelError("error", "some error");
            var model = new UserCreateFormViewModel { Role = UserRole.Admin };

            // Act
            var result = await _controller.Create(model);

            // Assert
            var viewResult = result as ViewResult;
            Assert.That(viewResult, Is.Not.Null);
            Assert.That(viewResult!.Model, Is.EqualTo(model));
        }

        [Test]
        public async Task Create_Post_UploadsFile_InsertsUser_AddsRole_AndRedirects()
        {
            // Arrange
            var fileContent = new MemoryStream(Encoding.UTF8.GetBytes("fake image content"));
            var formFile = new FormFile(fileContent, 0, fileContent.Length, "File", "profile.jpg")
            {
                Headers = new HeaderDictionary(),
                ContentType = "image/jpeg"
            };

            var viewModel = new UserCreateFormViewModel
            {
                Email = "test@example.com",
                Password = "SecurePassword123!",
                Role = UserRole.Admin,
                File = formFile // ✅ This is what triggers file upload in the controller
            };

            var generatedVirtualPath = "fake/path/profile.jpg";
            var newUser = new User
            {
                Email = viewModel.Email
            };

            _mapperMock
                .Setup(m => m.Map<UserCreateFormViewModel, User>(viewModel))
                .Returns(newUser);

            _fileHandlerMock
                .Setup(f => f.GetRelativePath("profile.jpg", It.IsAny<string>(), FileType.Profile))
                .Returns(generatedVirtualPath);

            _fileHandlerMock
                .Setup(f => f.Upload(formFile, generatedVirtualPath))
                .ReturnsAsync(true);

            _unitOfWorkMock
                .Setup(u => u.UserRepository.Insert(newUser, viewModel.Password))
                .ReturnsAsync(IdentityResult.Success);

            _unitOfWorkMock
                .Setup(u => u.UserRepository.AddToRole(newUser, viewModel.Role.ToString()))
                .ReturnsAsync(IdentityResult.Success);

            _unitOfWorkMock.Setup(u => u.Complete());

            // Act
            var result = await _controller.Create(viewModel);

            // Assert
            var redirectResult = result as RedirectToActionResult;
            Assert.IsNotNull(redirectResult);
            Assert.AreEqual("Index", redirectResult.ActionName);
            Assert.IsNull(redirectResult.ControllerName);

            _fileHandlerMock.Verify(f => f.Upload(formFile, generatedVirtualPath), Times.Once);
            _unitOfWorkMock.Verify(u => u.UserRepository.Insert(newUser, viewModel.Password), Times.Once);
            _unitOfWorkMock.Verify(u => u.UserRepository.AddToRole(newUser, viewModel.Role.ToString()), Times.Once);
        }

        // Helper class to mock antiforgery validation
        private class FakeAntiforgeryValidationFeature : IAntiforgeryValidationFeature
        {
            public bool IsValid => true;

            public Exception? Error => null;
        }

        [Test]
        public async Task Edit_Get_UserNotFound_ReturnsNotFound()
        {
            // Arrange
            _unitOfWorkMock.Setup(u => u.UserRepository.GetById(It.IsAny<int>())).ReturnsAsync((User?)null);

            // Act
            var result = await _controller.Edit(123);

            // Assert
            Assert.That(result, Is.TypeOf<NotFoundResult>());
        }

        [Test]
        public async Task Edit_Get_UserNotAllowed_ReturnsUnauthorized()
        {
            // Arrange
            var user = new User { Id = 2 };
            var currentUser = new User { Id = 1 };

            _unitOfWorkMock.Setup(u => u.UserRepository.GetById(It.IsAny<int>())).ReturnsAsync(user);
            _unitOfWorkMock.Setup(u => u.UserRepository.GetByClaimsPrincipal(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(currentUser);
            _unitOfWorkMock.Setup(u => u.UserRepository.IsUserAllowedForOperation(currentUser, user.Id, UserRolesConstant.SuperAdmin))
                .ReturnsAsync(false);

            // Act
            var result = await _controller.Edit(2);

            // Assert
            Assert.That(result, Is.TypeOf<UnauthorizedResult>());
        }

        [Test]
        public async Task Edit_Get_UserAllowed_ReturnsViewWithMappedModel()
        {
            // Arrange
            var user = new User { Id = 2 };
            var currentUser = new User { Id = 1 };
            var mappedVm = new UserFormViewModel { Id = 2 };

            _unitOfWorkMock.Setup(u => u.UserRepository.GetById(It.IsAny<int>())).ReturnsAsync(user);
            _unitOfWorkMock.Setup(u => u.UserRepository.GetByClaimsPrincipal(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(currentUser);
            _unitOfWorkMock.Setup(u => u.UserRepository.IsUserAllowedForOperation(currentUser, user.Id, UserRolesConstant.SuperAdmin))
                .ReturnsAsync(true);
            _mapperMock.Setup(m => m.Map<User, UserFormViewModel>(user)).Returns(mappedVm);

            // Act
            var result = await _controller.Edit(2);

            // Assert
            var viewResult = result as ViewResult;
            Assert.That(viewResult, Is.Not.Null);
            Assert.That(viewResult!.Model, Is.EqualTo(mappedVm));
        }

        [Test]
        public async Task Edit_Post_ModelStateInvalid_ReturnsViewWithModel()
        {
            // Arrange
            _controller.ModelState.AddModelError("error", "some error");
            var model = new UserFormViewModel { Id = 1 };

            // Act
            var result = await _controller.Edit(model.Id, model);

            // Assert
            var viewResult = result as ViewResult;
            Assert.That(viewResult, Is.Not.Null);
            Assert.That(viewResult!.Model, Is.EqualTo(model));
        }

        [Test]
        public async Task Edit_Post_UserNotFound_ReturnsNotFound()
        {
            // Arrange
            _unitOfWorkMock.Setup(u => u.UserRepository.GetById(It.IsAny<int>())).ReturnsAsync((User?)null);
            var model = new UserFormViewModel { Id = 1 };

            // Act
            var result = await _controller.Edit(model.Id, model);

            // Assert
            Assert.That(result, Is.TypeOf<NotFoundResult>());
        }

        [Test]
        public async Task Edit_Post_UserNotAllowed_ReturnsUnauthorized()
        {
            // Arrange
            var user = new User { Id = 2 };
            var currentUser = new User { Id = 1 };

            _unitOfWorkMock.Setup(u => u.UserRepository.GetById(user.Id)).ReturnsAsync(user);
            _unitOfWorkMock.Setup(u => u.UserRepository.GetByClaimsPrincipal(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(currentUser);
            _unitOfWorkMock.Setup(u => u.UserRepository.IsUserAllowedForOperation(currentUser, user.Id, UserRolesConstant.SuperAdmin))
                .ReturnsAsync(false);

            var model = new UserFormViewModel { Id = 2 };

            // Act
            var result = await _controller.Edit(model.Id, model);

            // Assert
            Assert.That(result, Is.TypeOf<UnauthorizedResult>());
        }

        [Test]
        public async Task Edit_Post_UpdatesUser_AndRedirects()
        {
            // Arrange
            var user = new User { Id = 2 };
            var currentUser = new User { Id = 1 };
            var model = new UserFormViewModel
            {
                Id = 2,
                UserName = "newuser",
                FirstName = "First",
                LastName = "Last",
                Email = "new@example.com",
                Biography = "Bio",
                Role = UserRole.Admin,
                IsActive = true,
                Password = "Password123!"
            };

            _unitOfWorkMock.Setup(u => u.UserRepository.GetById(model.Id)).ReturnsAsync(user);
            _unitOfWorkMock.Setup(u => u.UserRepository.GetByClaimsPrincipal(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(currentUser);
            _unitOfWorkMock.Setup(u => u.UserRepository.IsUserAllowedForOperation(currentUser, user.Id, UserRolesConstant.SuperAdmin))
                .ReturnsAsync(true);

            // No mapper involved, so no _mapperMock.Setup needed

            // Setup file collection to empty so that file upload is skipped (simplifies test)
            _controller.ControllerContext = new ControllerContext();
            _controller.ControllerContext.HttpContext = new DefaultHttpContext();
            _controller.ControllerContext.HttpContext.Request.Form = new FormCollection(null, new FormFileCollection());

            // Setup Update to return success
            _unitOfWorkMock.Setup(u => u.UserRepository.Update(user, model.Password))
                .ReturnsAsync(IdentityResult.Success);

            // Setup SignOut
            _unitOfWorkMock.Setup(u => u.UserRepository.SignOut())
                .Returns(Task.CompletedTask)
                .Verifiable();

            // Act
            var result = await _controller.Edit(model.Id, model);

            // Assert
            var redirect = result as RedirectToActionResult;
            Assert.That(redirect, Is.Not.Null);
            Assert.That(redirect!.ActionName, Is.EqualTo("Index"));

            _unitOfWorkMock.Verify(u => u.UserRepository.Update(user, model.Password), Times.Once);
            _unitOfWorkMock.Verify(u => u.UserRepository.SignOut(), Times.Once);
        }

        [Test]
        public async Task ToggleActive_UserNotFound_ReturnsBadRequest()
        {
            // Arrange
            _unitOfWorkMock.Setup(repo => repo.UserRepository.GetById(It.IsAny<int>())).ReturnsAsync((User)null);

            // Act
            var result = await _controller.ToggleActive(1);

            // Assert
            Assert.IsInstanceOf<JsonResult>(result);
            Assert.IsInstanceOf<BadRequestResult>(((JsonResult)result).Value);
        }

        [Test]
        public async Task ToggleActive_UserIsSuperAdmin_ReturnsBadRequest()
        {
            // Arrange
            var user = new User();
            _unitOfWorkMock.Setup(repo => repo.UserRepository.GetById(1)).ReturnsAsync(user);
            _unitOfWorkMock.Setup(repo => repo.UserRepository.IsInRole(user, UserRolesConstant.SuperAdmin)).ReturnsAsync(true);

            // Act
            var result = await _controller.ToggleActive(1);

            // Assert
            Assert.IsInstanceOf<JsonResult>(result);
            Assert.IsInstanceOf<BadRequestResult>(((JsonResult)result).Value);
        }

        [Test]
        public async Task ToggleActive_ValidUser_TogglesIsActiveAndReturnsOk()
        {
            // Arrange
            var user = new User { IsActive = false };
            _unitOfWorkMock.Setup(repo => repo.UserRepository.GetById(1)).ReturnsAsync(user);
            _unitOfWorkMock.Setup(repo => repo.UserRepository.IsInRole(user, UserRolesConstant.SuperAdmin)).ReturnsAsync(false);

            // Act
            var result = await _controller.ToggleActive(1);

            // Assert
            _unitOfWorkMock.Verify(r => r.UserRepository.Update(user, It.IsAny<string>()), Times.Once);
            Assert.IsTrue(user.IsActive);
            Assert.IsInstanceOf<JsonResult>(result);
            Assert.IsInstanceOf<OkResult>(((JsonResult)result).Value);
        }

        [Test]
        public void Login_Get_ReturnsViewResult()
        {
            // Act
            var result = _controller.Login("/home");

            // Assert
            Assert.IsInstanceOf<ViewResult>(result);
            Assert.AreEqual("/home", ((ViewResult)result).ViewData["returnUrl"]);
        }

        [Test]
        public async Task Login_Post_InvalidModelState_ReturnsView()
        {
            // Arrange
            _controller.ModelState.AddModelError("Username", "Required");

            var model = new UserLoginFormViewModel();

            // Act
            var result = await _controller.Login(model, "/home");

            // Assert
            Assert.IsInstanceOf<ViewResult>(result);
            Assert.AreEqual(model, ((ViewResult)result).Model);
        }

        [Test]
        public async Task Login_Post_UserNotFound_ReturnsViewWithModelError()
        {
            // Arrange
            var model = new UserLoginFormViewModel { Username = "user" };
            _unitOfWorkMock.Setup(r => r.UserRepository.GetByUserName("user")).ReturnsAsync((User)null);

            // Act
            var result = await _controller.Login(model, "/");

            // Assert
            Assert.IsInstanceOf<ViewResult>(result);
            Assert.IsTrue(_controller.ModelState.ContainsKey("Username"));
        }

        [Test]
        public async Task Login_Post_InactiveUser_ReturnsViewWithError()
        {
            // Arrange
            var user = new User { IsActive = false };
            var model = new UserLoginFormViewModel { Username = "user" };

            _unitOfWorkMock.Setup(r => r.UserRepository.GetByUserName("user")).ReturnsAsync(user);

            // Act
            var result = await _controller.Login(model, "/");

            // Assert
            Assert.IsInstanceOf<ViewResult>(result);
            Assert.AreEqual(MessagesConstant.IdentityAccountDisabled, ((ViewResult)result).ViewData["Error"]);
        }

        [Test]
        public async Task ForgotPassword_Post_ValidEmail_EmailSent_ReturnsRedirect()
        {
            // Arrange
            var user = new User { Id = 1, Email = "test@test.com" };
            var model = new UserForgotPasswordFormViewModel { Email = "test@test.com" };

            _controller.ModelState.Clear(); // Ensure valid model state

            _unitOfWorkMock.Setup(r => r.UserRepository.GetByEmail(model.Email)).ReturnsAsync(user);
            _unitOfWorkMock.Setup(r => r.UserRepository.GeneratePasswordResetToken(user)).ReturnsAsync("token");
            _emailSenderMock.Setup(e => e.SendEmail(model.Email, It.IsAny<string>(), It.IsAny<string>())).Returns(true);

            // Mock UrlHelper
            _controller.Url = Mock.Of<IUrlHelper>(u =>
                u.Action(It.IsAny<UrlActionContext>()) == "https://example.com/reset");

            // Act
            var result = await _controller.ForgotPassword(model);

            // Assert
            Assert.IsInstanceOf<RedirectToActionResult>(result);
            Assert.AreEqual("ForgotPasswordConfirmation", ((RedirectToActionResult)result).ActionName);
        }

        [Test]
        public async Task ForgotPassword_Post_UserNotFound_ReturnsViewWithError()
        {
            // Arrange
            var model = new UserForgotPasswordFormViewModel { Email = "notfound@test.com" };
            _unitOfWorkMock.Setup(r => r.UserRepository.GetByEmail(model.Email)).ReturnsAsync((User)null);

            // Act
            var result = await _controller.ForgotPassword(model);

            // Assert
            Assert.IsInstanceOf<ViewResult>(result);
            Assert.AreEqual(MessagesConstant.IdentityNotRegisteredYet, ((ViewResult)result).ViewData["Error"]);
        }

        [Test]
        public async Task Logout_Post_CallsSignOutAndRedirects()
        {
            // Arrange
            _unitOfWorkMock.Setup(r => r.UserRepository.SignOut());

            // Act
            var result = await _controller.Logout();

            // Assert
            _unitOfWorkMock.Verify(r => r.UserRepository.SignOut(), Times.Once);
            Assert.IsInstanceOf<RedirectToActionResult>(result);
            Assert.AreEqual("Index", ((RedirectToActionResult)result).ActionName);
        }

        [Test]
        public void ResetPassword_Get_ReturnsViewWithToken()
        {
            // Act
            var result = _controller.ResetPassword("token") as ViewResult;
            var model = result?.Model as UserResetPasswordFormViewModel;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("token", model?.Token);
        }

        [Test]
        public async Task ResetPassword_Post_InvalidModel_ReturnsView()
        {
            // Arrange
            _controller.ModelState.AddModelError("Password", "Required");
            var model = new UserResetPasswordFormViewModel();

            // Act
            var result = await _controller.ResetPassword(model);

            // Assert
            Assert.IsInstanceOf<ViewResult>(result);
        }

        [Test]
        public async Task ResetPassword_Post_UserNotFound_RedirectsToConfirmation()
        {
            // Arrange
            var model = new UserResetPasswordFormViewModel { Email = "notfound@test.com" };
            _unitOfWorkMock.Setup(r => r.UserRepository.GetByEmail(model.Email)).ReturnsAsync((User)null);

            // Act
            var result = await _controller.ResetPassword(model);

            // Assert
            Assert.IsInstanceOf<RedirectToActionResult>(result);
            Assert.AreEqual("ResetPasswordConfirmation", ((RedirectToActionResult)result).ActionName);
        }

        [Test]
        public async Task ResetPassword_Post_SuccessfulReset_Redirects()
        {
            // Arrange
            var user = new User { Email = "user@test.com" };
            var model = new UserResetPasswordFormViewModel { Email = user.Email, Token = "token", Password = "pass" };

            _unitOfWorkMock.Setup(r => r.UserRepository.GetByEmail(user.Email)).ReturnsAsync(user);
            _unitOfWorkMock.Setup(r => r.UserRepository.ResetPassword(user, model.Token, model.Password)).ReturnsAsync(IdentityResult.Success);

            // Act
            var result = await _controller.ResetPassword(model);

            // Assert
            Assert.IsInstanceOf<RedirectToActionResult>(result);
            Assert.AreEqual("ResetPasswordConfirmation", ((RedirectToActionResult)result).ActionName);
        }
    }
}
