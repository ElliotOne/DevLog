using System.Net;
using AutoMapper;
using DevLog.Controllers;
using DevLog.Core.Domain;
using DevLog.Models.Constants;
using DevLog.Models.Shared.JsonResults;
using DevLog.Models.ViewModels;
using DevLog.Persistence;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace DevLog.UnitTests.Controllers
{
    [TestFixture]
    public class HomeControllerTests
    {
        private Mock<IUnitOfWork> _unitOfWorkMock;
        private Mock<IMapper> _mapperMock;
        private Mock<IHttpContextAccessor> _httpContextAccessorMock;
        private HomeController _controller;

        [SetUp]
        public void SetUp()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _mapperMock = new Mock<IMapper>();
            _httpContextAccessorMock = new Mock<IHttpContextAccessor>();

            _controller = new HomeController(
                _unitOfWorkMock.Object,
                _mapperMock.Object,
                _httpContextAccessorMock.Object);
        }

        [Test]
        public void Index_ReturnsView()
        {
            // Act
            var result = _controller.Index();

            // Assert
            Assert.IsInstanceOf<ViewResult>(result);
        }

        [Test]
        public void Contact_Get_ReturnsView()
        {
            // Act
            var result = _controller.Contact();

            // Assert
            Assert.IsInstanceOf<ViewResult>(result);
        }

        [Test]
        public async Task Contact_Post_InvalidModelState_ReturnsModelErrorJson()
        {
            // Arrange
            _controller.ModelState.AddModelError("Email", "Required");

            // Act
            var result = await _controller.Contact(new ContactFormViewModel());

            // Assert
            Assert.IsInstanceOf<JsonResult>(result);
            var json = result as JsonResult;
            var data = json?.Value as JsonResultModel;

            Assert.AreEqual(JsonResultStatusCode.ModelStateIsNotValid, data?.StatusCode);
            Assert.AreEqual(MessagesConstant.ContactFailedToSend, data?.Message);
        }

        [Test]
        public async Task Contact_Post_ValidModelState_ReturnsSuccessJson()
        {
            // Arrange
            var model = new ContactFormViewModel
            {
                UserFullName = "Test",
                Subject = "Test",
                EmailOrPhoneNumber = "test@example.com",
                Body = "Hello"
            };
            var contact = new Contact();

            _mapperMock.Setup(m => m.Map<ContactFormViewModel, Contact>(model)).Returns(contact);

            var mockContext = new DefaultHttpContext();
            mockContext.Connection.RemoteIpAddress = IPAddress.Parse("192.168.1.1");
            _httpContextAccessorMock.Setup(h => h.HttpContext).Returns(mockContext);

            _unitOfWorkMock.Setup(u => u.ContactRepository.Insert(contact));
            _unitOfWorkMock.Setup(u => u.Complete()).Returns(Task.CompletedTask);

            // Act
            var result = await _controller.Contact(model);

            // Assert
            Assert.IsInstanceOf<JsonResult>(result);
            var json = result as JsonResult;
            var data = json?.Value as JsonResultModel;

            Assert.AreEqual(JsonResultStatusCode.Success, data?.StatusCode);
            Assert.AreEqual(MessagesConstant.ContactSentSuccessfully, data?.Message);
        }

        [Test]
        public void About_ReturnsView()
        {
            // Act
            var result = _controller.About();

            // Assert
            Assert.IsInstanceOf<ViewResult>(result);
        }

        [Test]
        public async Task Donate_ReturnsViewWithViewModel()
        {
            // Arrange
            var settings = new Setting();
            var donateVm = new DonateViewModel();

            _unitOfWorkMock.Setup(u => u.SettingRepository.Get()).ReturnsAsync(settings);
            _mapperMock.Setup(m => m.Map<Setting, DonateViewModel>(settings)).Returns(donateVm);

            // Act
            var result = await _controller.Donate();

            // Assert
            Assert.IsInstanceOf<ViewResult>(result);
            var viewModel = (result as ViewResult)?.Model as DonateViewModel;
            Assert.NotNull(viewModel);
        }
    }
}