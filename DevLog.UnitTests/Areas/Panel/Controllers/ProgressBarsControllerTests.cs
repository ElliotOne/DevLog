using AutoMapper;
using DevLog.Areas.Panel.Controllers;
using DevLog.Areas.Panel.Models.ViewModels;
using DevLog.Core.Domain;
using DevLog.Models.Shared.JsonResults;
using DevLog.Persistence;
using DevLog.UnitTests.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Moq;

namespace DevLog.UnitTests.Areas.Panel.Controllers
{
    [TestFixture]
    public class ProgressBarsControllerTests
    {
        private Mock<IUnitOfWork> _unitOfWorkMock;
        private Mock<IMapper> _mapperMock;
        private ProgressBarsController _controller;

        [SetUp]
        public void SetUp()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _mapperMock = new Mock<IMapper>();

            _controller = new ProgressBarsController(_unitOfWorkMock.Object, _mapperMock.Object);
        }

        [Test]
        public async Task Index_Get_ReturnsViewWithModel()
        {
            // Arrange
            var progressBars = new List<ProgressBar>
            {
                new ProgressBar { Id = 1, SortIndex = 2 },
                new ProgressBar { Id = 2, SortIndex = 1 }
            };

            var asyncQueryable = new TestAsyncEnumerable<ProgressBar>(progressBars);

            _unitOfWorkMock.Setup(u => u.ProgressBarRepository.GetAll()).Returns(asyncQueryable);

            var mappedViewModels = new List<ProgressBarFormViewModel>
            {
                new ProgressBarFormViewModel { Id = 1 },
                new ProgressBarFormViewModel { Id = 2 }
            };

            _mapperMock
                .Setup(m => m.Map<IEnumerable<ProgressBar>, IList<ProgressBarFormViewModel>>(It.IsAny<IEnumerable<ProgressBar>>()))
                .Returns(mappedViewModels);

            // Act
            var result = await _controller.Index();

            // Assert
            Assert.IsInstanceOf<ViewResult>(result);
            var viewResult = (ViewResult)result;
            Assert.IsInstanceOf<ProgressBarIndexViewModel>(viewResult.Model);

            var model = (ProgressBarIndexViewModel)viewResult.Model;
            Assert.AreEqual(mappedViewModels, model.ProgressBarFormViewModels);
        }

        [Test]
        public async Task Index_Post_IdMismatch_ReturnsBadRequest()
        {
            // Arrange
            var vm = new ProgressBarIndexViewModel { Id = 2 };

            // Act
            var result = await _controller.Index(1, vm);

            // Assert
            Assert.IsInstanceOf<BadRequestResult>(result);
        }

        [Test]
        public async Task Index_Post_InvalidModel_ReturnsViewWithModel()
        {
            // Arrange
            _controller.ModelState.AddModelError("Error", "Invalid model");

            var vm = new ProgressBarIndexViewModel
            {
                Id = 1,
                ProgressBarFormViewModels = new List<ProgressBarFormViewModel>()
            };

            // Act
            var result = await _controller.Index(1, vm);

            // Assert
            Assert.IsInstanceOf<ViewResult>(result);
            var viewResult = (ViewResult)result;
            Assert.AreEqual(vm, viewResult.Model);
        }

        [Test]
        public async Task Index_Post_ValidModel_UpdatesAndRedirects()
        {
            // Arrange
            var vm = new ProgressBarIndexViewModel
            {
                Id = 1,
                ProgressBarFormViewModels = new List<ProgressBarFormViewModel>
                {
                    new ProgressBarFormViewModel { Id = 1 },
                    new ProgressBarFormViewModel { Id = 2 }
                }
            };

            var mappedEntities = new List<ProgressBar>
            {
                new ProgressBar { Id = 1 },
                new ProgressBar { Id = 2 }
            };

            _mapperMock
                .Setup(m => m.Map<IList<ProgressBarFormViewModel>, IEnumerable<ProgressBar>>(vm.ProgressBarFormViewModels))
                .Returns(mappedEntities);

            _unitOfWorkMock.Setup(u => u.ProgressBarRepository.UpdateAll(
                It.IsAny<IEnumerable<ProgressBar>>())
            );

            // Act
            var result = await _controller.Index(vm.Id, vm);

            // Assert
            _unitOfWorkMock.Verify(u => u.ProgressBarRepository.UpdateAll(mappedEntities), Times.Once);
            _unitOfWorkMock.Verify(u => u.Complete(), Times.Once);

            Assert.IsInstanceOf<RedirectToActionResult>(result);
            var redirectResult = (RedirectToActionResult)result;
            Assert.AreEqual("Index", redirectResult.ActionName);
        }

        [Test]
        public void Create_Get_ReturnsView()
        {
            // Act
            var result = _controller.Create();

            // Assert
            Assert.IsInstanceOf<ViewResult>(result);
        }

        [Test]
        public async Task Create_Post_InvalidModel_ReturnsViewWithModel()
        {
            // Arrange
            _controller.ModelState.AddModelError("Error", "Invalid");

            var vm = new ProgressBarFormViewModel { Topic = "Test" };

            // Act
            var result = await _controller.Create(vm);

            // Assert
            Assert.IsInstanceOf<ViewResult>(result);
            var viewResult = (ViewResult)result;
            Assert.AreEqual(vm, viewResult.Model);
        }

        [Test]
        public async Task Create_Post_ValidModel_InsertsAndRedirects()
        {
            // Arrange
            var vm = new ProgressBarFormViewModel { Topic = "Test" };
            var domainModel = new ProgressBar { Topic = "Test" };

            _unitOfWorkMock.Setup(u => u.ProgressBarRepository.Insert(It.IsAny<ProgressBar>()));
            _mapperMock.Setup(m => m.Map<ProgressBarFormViewModel, ProgressBar>(vm)).Returns(domainModel);

            // Act
            var result = await _controller.Create(vm);

            // Assert
            _unitOfWorkMock.Verify(r => r.ProgressBarRepository.Insert(domainModel), Times.Once);
            _unitOfWorkMock.Verify(u => u.Complete(), Times.Once);

            Assert.IsInstanceOf<RedirectToActionResult>(result);
            var redirectResult = (RedirectToActionResult)result;
            Assert.AreEqual("Index", redirectResult.ActionName);
        }

        [Test]
        public async Task Delete_ReturnsJsonResultAndDeletes()
        {
            // Arrange
            int progressBarId = 1;

            _unitOfWorkMock.Setup(repo => repo.ProgressBarRepository.Delete(progressBarId)).Returns(Task.CompletedTask);
            _unitOfWorkMock.Setup(uow => uow.Complete()).Returns(Task.CompletedTask);

            var mockUrlHelper = new Mock<IUrlHelper>();
            mockUrlHelper.Setup(u => u.Action(It.IsAny<UrlActionContext>()))
                .Returns("/Panel/ProgressBars/Index");

            _controller.Url = mockUrlHelper.Object;

            // Act
            var result = await _controller.Delete(progressBarId);

            // Assert
            Assert.IsInstanceOf<JsonResult>(result);
            var jsonResult = result as JsonResult;
            Assert.IsNotNull(jsonResult);
            Assert.IsInstanceOf<JsonResultModel>(jsonResult.Value);

            var jsonModel = jsonResult.Value as JsonResultModel;
            Assert.AreEqual(JsonResultStatusCode.Success, jsonModel.StatusCode);
            Assert.IsNotNull(jsonModel.RedirectUrl);
            Assert.That(jsonModel.RedirectUrl, Does.Contain("ProgressBars"));

            _unitOfWorkMock.Verify(repo => repo.ProgressBarRepository.Delete(progressBarId), Times.Once);
            _unitOfWorkMock.Verify(uow => uow.Complete(), Times.Once);
        }
    }
}
