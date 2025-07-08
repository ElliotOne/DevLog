using AutoMapper;
using DevLog.Areas.Panel.Controllers;
using DevLog.Areas.Panel.Models.ViewModels;
using DevLog.Core.Domain;
using DevLog.Persistence;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace DevLog.UnitTests.Areas.Panel.Controllers
{
    [TestFixture]
    public class SettingsControllerTests
    {
        private Mock<IUnitOfWork> _unitOfWorkMock;
        private Mock<IMapper> _mapperMock;
        private SettingsController _controller;

        [SetUp]
        public void Setup()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _mapperMock = new Mock<IMapper>();
            _controller = new SettingsController(_unitOfWorkMock.Object, _mapperMock.Object);
        }

        [Test]
        public async Task Index_WhenSettingExists_ReturnsViewWithMappedModel()
        {
            // Arrange
            var setting = new Setting { Id = 1 };
            var viewModel = new SettingFormViewModel { Id = 1 };

            _unitOfWorkMock.Setup(u => u.SettingRepository.Get()).ReturnsAsync(setting);
            _mapperMock.Setup(m => m.Map<Setting, SettingFormViewModel>(setting)).Returns(viewModel);

            // Act
            var result = await _controller.Index();

            // Assert
            Assert.IsInstanceOf<ViewResult>(result);
            var viewResult = (ViewResult)result;
            Assert.AreEqual(viewModel, viewResult.Model);
        }

        [Test]
        public async Task Index_WhenSettingIsNull_ReturnsNotFound()
        {
            // Arrange
            _unitOfWorkMock.Setup(u => u.SettingRepository.Get()).ReturnsAsync((Setting)null);

            // Act
            var result = await _controller.Index();

            // Assert
            Assert.IsInstanceOf<NotFoundResult>(result);
        }

        [Test]
        public async Task IndexPost_WhenIdDoesNotMatchModel_ReturnsBadRequest()
        {
            // Arrange
            var vm = new SettingFormViewModel { Id = 2 };

            // Act
            var result = await _controller.IndexPost(1, vm);

            // Assert
            Assert.IsInstanceOf<BadRequestResult>(result);
        }

        [Test]
        public async Task IndexPost_WhenModelStateIsInvalid_ReturnsViewWithModel()
        {
            // Arrange
            var vm = new SettingFormViewModel { Id = 1 };
            _controller.ModelState.AddModelError("Error", "Invalid model");

            // Act
            var result = await _controller.IndexPost(1, vm);

            // Assert
            Assert.IsInstanceOf<ViewResult>(result);
            var viewResult = (ViewResult)result;
            Assert.AreEqual(vm, viewResult.Model);
        }

        [Test]
        public async Task IndexPost_WhenModelIsValid_UpdatesSettingAndRedirects()
        {
            // Arrange
            var vm = new SettingFormViewModel { Id = 1 };
            var domainModel = new Setting { Id = 1 };

            _mapperMock.Setup(m => m.Map<SettingFormViewModel, Setting>(vm)).Returns(domainModel);

            _unitOfWorkMock.Setup(u => u.SettingRepository.Update(It.IsAny<Setting>()));

            // Act
            var result = await _controller.IndexPost(1, vm);

            // Assert
            _unitOfWorkMock.Verify(u => u.SettingRepository.Update(domainModel), Times.Once);
            _unitOfWorkMock.Verify(u => u.Complete(), Times.Once);

            Assert.IsInstanceOf<RedirectToActionResult>(result);
            var redirectResult = (RedirectToActionResult)result;
            Assert.AreEqual(nameof(SettingsController.Index), redirectResult.ActionName);
        }
    }
}
