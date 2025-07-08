using AutoMapper;
using DevLog.Areas.Panel.Controllers;
using DevLog.Areas.Panel.Models;
using DevLog.Areas.Panel.Models.ViewModels;
using DevLog.Core.Domain;
using DevLog.Persistence;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace DevLog.UnitTests.Areas.Panel.Controllers
{
    [TestFixture]
    public class ContactsControllerTests
    {
        private Mock<IUnitOfWork> _unitOfWorkMock;
        private Mock<IMapper> _mapperMock;
        private ContactsController _controller;

        [SetUp]
        public void Setup()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _mapperMock = new Mock<IMapper>();
            _controller = new ContactsController(_unitOfWorkMock.Object, _mapperMock.Object);
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
        public async Task LoadContactsTable_WithSearchAndOrdering_ReturnsJsonResult()
        {
            // Arrange
            var contacts = new List<Contact>
            {
                new Contact { Id = 1, UserFullName = "Alice", EmailOrPhoneNumber = "alice@test.com", Subject = "Hello", Body = "Body1", Ip = "127.0.0.1", CreateDate = DateTime.Now, LastEditDate = DateTime.Now },
                new Contact { Id = 2, UserFullName = "Bob", EmailOrPhoneNumber = "bob@test.com", Subject = "Hi", Body = "Body2", Ip = "192.168.0.1", CreateDate = DateTime.Now, LastEditDate = DateTime.Now }
            }.AsQueryable();

            var dtParameters = new DTParameters
            {
                Draw = 1,
                Start = 0,
                Length = 10,
                Search = new DTSearch { Value = "Alice" },
                Order = new DTOrder[] { new DTOrder { Column = 0, Dir = DTOrderDir.ASC } },
                Columns = new DTColumn[] { new DTColumn { Data = nameof(Contact.UserFullName) } }
            };

            _unitOfWorkMock.Setup(u => u.ContactRepository.GetAll()).Returns(contacts);
            _unitOfWorkMock.Setup(u => u.ContactRepository.Count()).ReturnsAsync(contacts.Count());

            var mappedResult = new List<ContactIndexViewModel> { new ContactIndexViewModel { UserFullName = "Alice" } };
            _mapperMock.Setup(m => m.Map<IEnumerable<Contact>, IEnumerable<ContactIndexViewModel>>(It.IsAny<IEnumerable<Contact>>()))
                .Returns(mappedResult);

            // Act
            var result = await _controller.LoadContactsTable(dtParameters);

            // Assert
            Assert.IsInstanceOf<JsonResult>(result);
            var jsonResult = (JsonResult)result;
            var data = (DatatableJsonResultModel)jsonResult.Value;
            Assert.AreEqual(1, data.Draw);
            Assert.AreEqual(contacts.Count(), data.RecordsTotal);
            Assert.AreEqual(1, data.RecordsFiltered);
            Assert.AreEqual(mappedResult, data.Data);
        }

        [Test]
        public async Task Details_ExistingContact_ReturnsViewWithModel()
        {
            // Arrange
            var contact = new Contact { Id = 1 };
            var viewModel = new ContactFormViewModel { Id = 1 };

            _unitOfWorkMock.Setup(u => u.ContactRepository.GetById(1)).ReturnsAsync(contact);
            _mapperMock.Setup(m => m.Map<Contact, ContactFormViewModel>(contact)).Returns(viewModel);

            // Act
            var result = await _controller.Details(1);

            // Assert
            Assert.IsInstanceOf<ViewResult>(result);
            var viewResult = (ViewResult)result;
            Assert.AreEqual(viewModel, viewResult.Model);
        }

        [Test]
        public async Task Details_NonExistingContact_ReturnsNotFound()
        {
            // Arrange
            _unitOfWorkMock.Setup(u => u.ContactRepository.GetById(1)).ReturnsAsync((Contact)null);

            // Act
            var result = await _controller.Details(1);

            // Assert
            Assert.IsInstanceOf<NotFoundResult>(result);
        }

        [Test]
        public async Task Edit_Get_ExistingContact_ReturnsViewWithModel()
        {
            // Arrange
            var contact = new Contact { Id = 1 };
            var viewModel = new ContactFormViewModel { Id = 1 };

            _unitOfWorkMock.Setup(u => u.ContactRepository.GetById(1)).ReturnsAsync(contact);
            _mapperMock.Setup(m => m.Map<Contact, ContactFormViewModel>(contact)).Returns(viewModel);

            // Act
            var result = await _controller.Edit(1);

            // Assert
            Assert.IsInstanceOf<ViewResult>(result);
            var viewResult = (ViewResult)result;
            Assert.AreEqual(viewModel, viewResult.Model);
        }

        [Test]
        public async Task Edit_Get_NonExistingContact_ReturnsNotFound()
        {
            // Arrange
            _unitOfWorkMock.Setup(u => u.ContactRepository.GetById(1)).ReturnsAsync((Contact)null);

            // Act
            var result = await _controller.Edit(1);

            // Assert
            Assert.IsInstanceOf<NotFoundResult>(result);
        }

        [Test]
        public async Task Edit_Post_IdMismatch_ReturnsBadRequest()
        {
            // Arrange
            var model = new ContactFormViewModel { Id = 2 };

            // Act
            var result = await _controller.Edit(1, model);

            // Assert
            Assert.IsInstanceOf<BadRequestResult>(result);
        }

        [Test]
        public async Task Edit_Post_InvalidModelState_ReturnsViewWithModel()
        {
            // Arrange
            var model = new ContactFormViewModel { Id = 1 };
            _controller.ModelState.AddModelError("Error", "Invalid");

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
            var model = new ContactFormViewModel { Id = 1 };
            var contact = new Contact { Id = 1 };

            _mapperMock.Setup(m => m.Map<ContactFormViewModel, Contact>(model)).Returns(contact);
            _unitOfWorkMock.Setup(u => u.ContactRepository.Update(contact));
            _unitOfWorkMock.Setup(u => u.Complete()).Returns(Task.CompletedTask);

            // Act
            var result = await _controller.Edit(1, model);

            _unitOfWorkMock.Verify(u => u.ContactRepository.Update(contact), Times.Once);
            _unitOfWorkMock.Verify(u => u.Complete(), Times.Once);

            // Assert
            Assert.IsInstanceOf<RedirectToActionResult>(result);
            var redirect = (RedirectToActionResult)result;
            Assert.AreEqual(nameof(_controller.Index), redirect.ActionName);
        }

        [Test]
        public async Task Delete_Get_ExistingContact_ReturnsViewWithModel()
        {
            // Arrange
            var contact = new Contact { Id = 1 };
            var viewModel = new ContactFormViewModel { Id = 1 };

            _unitOfWorkMock.Setup(u => u.ContactRepository.GetById(1)).ReturnsAsync(contact);
            _mapperMock.Setup(m => m.Map<Contact, ContactFormViewModel>(contact)).Returns(viewModel);

            // Act
            var result = await _controller.Delete(1);

            // Assert
            Assert.IsInstanceOf<ViewResult>(result);
            var viewResult = (ViewResult)result;
            Assert.AreEqual(viewModel, viewResult.Model);
        }

        [Test]
        public async Task Delete_Get_NonExistingContact_ReturnsNotFound()
        {
            // Arrange
            _unitOfWorkMock.Setup(u => u.ContactRepository.GetById(1)).ReturnsAsync((Contact)null);

            // Act
            var result = await _controller.Delete(1);

            // Assert
            Assert.IsInstanceOf<NotFoundResult>(result);
        }

        [Test]
        public async Task DeleteConfirmed_Post_DeletesAndRedirects()
        {
            // Arrange
            _unitOfWorkMock.Setup(u => u.ContactRepository.Delete(1)).Returns(Task.CompletedTask);
            _unitOfWorkMock.Setup(u => u.Complete()).Returns(Task.CompletedTask);

            // Act
            var result = await _controller.DeleteConfirmed(1);

            // Assert
            _unitOfWorkMock.Verify(u => u.ContactRepository.Delete(1), Times.Once);
            _unitOfWorkMock.Verify(u => u.Complete(), Times.Once);

            Assert.IsInstanceOf<RedirectToActionResult>(result);
            var redirect = (RedirectToActionResult)result;
            Assert.AreEqual(nameof(_controller.Index), redirect.ActionName);
        }
    }
}
