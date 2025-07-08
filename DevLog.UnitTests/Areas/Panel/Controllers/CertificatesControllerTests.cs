using AutoMapper;
using DevLog.Areas.Panel.Controllers;
using DevLog.Areas.Panel.Models;
using DevLog.Areas.Panel.Models.ViewModels;
using DevLog.Core.Domain;
using DevLog.Persistence;
using DevLog.Services.FileHandler;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace DevLog.UnitTests.Areas.Panel.Controllers
{
    [TestFixture]
    public class CertificatesControllerTests
    {
        private Mock<IUnitOfWork> _unitOfWorkMock;
        private Mock<IMapper> _mapperMock;
        private Mock<IFileHandler> _fileHandlerMock;
        private CertificatesController _controller;

        [SetUp]
        public void Setup()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _mapperMock = new Mock<IMapper>();
            _fileHandlerMock = new Mock<IFileHandler>();

            _controller = new CertificatesController(
                _unitOfWorkMock.Object,
                _mapperMock.Object,
                _fileHandlerMock.Object
            );

            // Setup dummy HttpContext for file upload tests
            var context = new DefaultHttpContext();
            context.Request.ContentType = "multipart/form-data";
            context.Request.Form = new FormCollection(new Dictionary<string, Microsoft.Extensions.Primitives.StringValues>(), new FormFileCollection());
            _controller.ControllerContext = new ControllerContext() { HttpContext = context };
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
        public async Task LoadCertificatesTable_ReturnsJsonResult_WithMappedData()
        {
            // Arrange
            var certificates = new List<Certificate>
            {
                new Certificate { Id = 1, Title = "Cert 1" },
                new Certificate { Id = 2, Title = "Cert 2" }
            }.AsQueryable();

            var dtParams = new DTParameters
            {
                Start = 0,
                Length = 10,
                Draw = 1,
                Order = new DTOrder[] { new DTOrder { Column = 0, Dir = DTOrderDir.ASC } },
                Columns = new DTColumn[] { new DTColumn { Data = "Title" } },
                Search = new DTSearch { Value = "" }
            };

            _unitOfWorkMock.Setup(u => u.CertificateRepository.GetAll()).Returns(certificates);
            _unitOfWorkMock.Setup(u => u.ContactRepository.Count()).ReturnsAsync(2);
            _mapperMock.Setup(m =>
                    m.Map<IEnumerable<Certificate>, IEnumerable<CertificateIndexViewModel>>(
                        It.IsAny<IEnumerable<Certificate>>()))
                .Returns(new List<CertificateIndexViewModel>());

            // Act
            var result = await _controller.LoadCertificatesTable(dtParams) as JsonResult;

            // Assert
            Assert.IsNotNull(result);
            var model = result.Value as DatatableJsonResultModel;
            Assert.IsNotNull(model);
            Assert.AreEqual(1, model.Draw);
            Assert.AreEqual(2, model.RecordsTotal);
            Assert.AreEqual(2, model.RecordsFiltered);
        }

        [Test]
        public async Task Details_WithExistingId_ReturnsViewWithMappedModel()
        {
            // Arrange
            var certificate = new Certificate { Id = 1, Title = "Cert" };
            var vm = new CertificateFormViewModel { Id = 1, Title = "Cert" };

            _unitOfWorkMock.Setup(u => u.CertificateRepository.GetById(1))
                .ReturnsAsync(certificate);

            _mapperMock.Setup(m => m.Map<Certificate, CertificateFormViewModel>(certificate))
                .Returns(vm);

            // Act
            var result = await _controller.Details(1);

            // Assert
            Assert.IsInstanceOf<ViewResult>(result);
            var viewResult = (ViewResult)result;
            Assert.AreEqual(vm, viewResult.Model);
        }

        [Test]
        public async Task Details_WithNonExistingId_ReturnsNotFound()
        {
            // Arrange
            _unitOfWorkMock.Setup(u => u.CertificateRepository.GetById(1))
                .ReturnsAsync((Certificate?)null);

            // Act
            var result = await _controller.Details(1);

            // Assert
            Assert.IsInstanceOf<NotFoundResult>(result);
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
            _controller.ModelState.AddModelError("error", "some error");
            var model = new CertificateFormViewModel();

            // Act
            var result = await _controller.Create(model);

            // Assert
            Assert.IsInstanceOf<ViewResult>(result);
            var viewResult = (ViewResult)result;
            Assert.AreEqual(model, viewResult.Model);
        }

        [Test]
        public async Task Create_Post_ValidModel_InsertsAndRedirects()
        {
            // Arrange
            var model = new CertificateFormViewModel
            {
                Title = "Test Certificate",
                // set other required properties as needed
            };

            var certificate = new Certificate();

            _mapperMock.Setup(m => m.Map<CertificateFormViewModel, Certificate>(It.IsAny<CertificateFormViewModel>()))
                .Returns(certificate);

            _unitOfWorkMock.Setup(u => u.CertificateRepository.Insert(It.IsAny<Certificate>()));

            _unitOfWorkMock.Setup(u => u.Complete())
                .Returns(Task.CompletedTask);

            // Act
            var result = await _controller.Create(model);

            // Assert
            _mapperMock.Verify(m => m.Map<CertificateFormViewModel, Certificate>(It.IsAny<CertificateFormViewModel>()), Times.Once);
            _unitOfWorkMock.Verify(u => u.CertificateRepository.Insert(It.Is<Certificate>(c => c == certificate)), Times.Once);
            _unitOfWorkMock.Verify(u => u.Complete(), Times.Once);

            Assert.IsInstanceOf<RedirectToActionResult>(result);
            var redirectResult = (RedirectToActionResult)result;
            Assert.AreEqual("Index", redirectResult.ActionName);
        }


        [Test]
        public async Task Edit_Get_WithExistingId_ReturnsViewWithModel()
        {
            // Arrange
            var certificate = new Certificate { Id = 1 };
            var vm = new CertificateFormViewModel { Id = 1 };

            _unitOfWorkMock.Setup(u => u.CertificateRepository.GetById(1))
                .ReturnsAsync(certificate);

            _mapperMock.Setup(m => m.Map<Certificate, CertificateFormViewModel>(certificate))
                .Returns(vm);

            // Act
            var result = await _controller.Edit(1);

            // Assert
            Assert.IsInstanceOf<ViewResult>(result);
            var viewResult = (ViewResult)result;
            Assert.AreEqual(vm, viewResult.Model);
        }

        [Test]
        public async Task Edit_Get_WithNonExistingId_ReturnsNotFound()
        {
            // Arrange
            _unitOfWorkMock.Setup(u => u.CertificateRepository.GetById(1))
                .ReturnsAsync((Certificate?)null);

            // Act
            var result = await _controller.Edit(1);

            // Assert
            Assert.IsInstanceOf<NotFoundResult>(result);
        }

        [Test]
        public async Task Edit_Post_IdMismatch_ReturnsBadRequest()
        {
            // Arrange
            var model = new CertificateFormViewModel { Id = 2 };

            // Act
            var result = await _controller.Edit(1, model);

            // Assert
            Assert.IsInstanceOf<BadRequestResult>(result);
        }

        [Test]
        public async Task Edit_Post_InvalidModel_ReturnsViewWithModel()
        {
            // Arrange
            var model = new CertificateFormViewModel { Id = 1 };
            _controller.ModelState.AddModelError("error", "some error");

            // Act
            var result = await _controller.Edit(1, model);

            // Assert
            Assert.IsInstanceOf<ViewResult>(result);
            var viewResult = (ViewResult)result;
            Assert.AreEqual(model, viewResult.Model);
        }

        [Test]
        public async Task Edit_Post_ValidModel_UpdatesAndRedirects()
        {
            // Arrange
            var model = new CertificateFormViewModel { Id = 1 };

            // The existing entity loaded from DB before update
            var existingCertificate = new Certificate
            {
                ImageVirtualPath = "somepath/to/image.jpg"
            };

            _unitOfWorkMock.Setup(u => u.CertificateRepository.GetById(1))
                .ReturnsAsync(existingCertificate);

            var updatedCertificate = new Certificate
            {
                ImageVirtualPath = null
            };
            _mapperMock.Setup(m => m.Map<CertificateFormViewModel, Certificate>(It.IsAny<CertificateFormViewModel>()))
                .Returns(updatedCertificate);

            _unitOfWorkMock.Setup(u => u.CertificateRepository.Update(It.IsAny<Certificate>()));
            _unitOfWorkMock.Setup(u => u.Complete()).Returns(Task.CompletedTask);

            _fileHandlerMock.Setup(f => f.Delete(It.IsAny<string>()));

            // Setup HttpContext with required form data to prevent errors in controller
            var formCollection = new FormCollection(new Dictionary<string, Microsoft.Extensions.Primitives.StringValues>(), new FormFileCollection());
            var context = new DefaultHttpContext();
            context.Request.ContentType = "multipart/form-data";
            context.Request.Form = formCollection;

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = context
            };

            // Act
            var result = await _controller.Edit(1, model);

            // Assert
            _unitOfWorkMock.Verify(u => u.CertificateRepository.Update(It.IsAny<Certificate>()), Times.Once);
            _unitOfWorkMock.Verify(u => u.Complete(), Times.Once);

            Assert.IsInstanceOf<RedirectToActionResult>(result);
            var redirect = (RedirectToActionResult)result;
            Assert.AreEqual("Index", redirect.ActionName);
        }

        [Test]
        public async Task Delete_Get_WithExistingId_ReturnsViewWithModel()
        {
            // Arrange
            var certificate = new Certificate { Id = 1 };
            var vm = new CertificateFormViewModel { Id = 1 };

            _unitOfWorkMock.Setup(u => u.CertificateRepository.GetById(1))
                .ReturnsAsync(certificate);

            _mapperMock.Setup(m => m.Map<Certificate, CertificateFormViewModel>(certificate))
                .Returns(vm);

            // Act
            var result = await _controller.Delete(1);

            // Assert
            Assert.IsInstanceOf<ViewResult>(result);
            var viewResult = (ViewResult)result;
            Assert.AreEqual(vm, viewResult.Model);
        }

        [Test]
        public async Task Delete_Get_WithNonExistingId_ReturnsNotFound()
        {
            // Arrange
            _unitOfWorkMock.Setup(u => u.CertificateRepository.GetById(1))
                .ReturnsAsync((Certificate?)null);

            // Act
            var result = await _controller.Delete(1);

            // Assert
            Assert.IsInstanceOf<NotFoundResult>(result);
        }

        [Test]
        public async Task DeleteConfirmed_WithNullImageVirtualPath_ReturnsBadRequest()
        {
            // Act
            var result = await _controller.DeleteConfirmed(1, null);

            // Assert
            Assert.IsInstanceOf<BadRequestResult>(result);
        }

        [Test]
        public async Task DeleteConfirmed_Valid_DeletesFileAndCertificate()
        {
            // Arrange
            _unitOfWorkMock.Setup(u => u.CertificateRepository.Delete(1)).Returns(Task.CompletedTask);
            _unitOfWorkMock.Setup(u => u.Complete()).Returns(Task.CompletedTask);

            // Act
            var result = await _controller.DeleteConfirmed(1, "somepath");

            // Assert
            _fileHandlerMock.Verify(f => f.Delete("somepath"), Times.Once);
            _unitOfWorkMock.Verify(u => u.CertificateRepository.Delete(1), Times.Once);
            _unitOfWorkMock.Verify(u => u.Complete(), Times.Once);

            Assert.IsInstanceOf<RedirectToActionResult>(result);
            var redirect = (RedirectToActionResult)result;
            Assert.AreEqual("Index", redirect.ActionName);
        }
    }
}