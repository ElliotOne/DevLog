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
    public class PostCategoriesControllerTests
    {
        private Mock<IUnitOfWork> _unitOfWorkMock;
        private Mock<IMapper> _mapperMock;
        private PostCategoriesController _controller;

        [SetUp]
        public void Setup()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _mapperMock = new Mock<IMapper>();
            _controller = new PostCategoriesController(_unitOfWorkMock.Object, _mapperMock.Object);
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
        public async Task LoadPostCategoriesTable_WithSearchAndOrdering_ReturnsJsonResult()
        {
            // Arrange
            var postCategories = new List<PostCategory>
            {
                new PostCategory { Id = 1, Title = "Category1", CreateDate = DateTime.Now, LastEditDate = DateTime.Now },
                new PostCategory { Id = 2, Title = "Category2", CreateDate = DateTime.Now, LastEditDate = DateTime.Now }
            }.AsQueryable();

            var dtParameters = new DTParameters
            {
                Draw = 1,
                Start = 0,
                Length = 10,
                Search = new DTSearch { Value = "Category1" },
                Order = new DTOrder[] { new DTOrder { Column = 0, Dir = DTOrderDir.ASC } },
                Columns = new DTColumn[] { new DTColumn { Data = nameof(PostCategory.Title) } }
            };

            _unitOfWorkMock.Setup(u => u.PostCategoryRepository.GetAll()).Returns(postCategories);
            _unitOfWorkMock.Setup(u => u.PostCategoryRepository.Count()).ReturnsAsync(postCategories.Count());

            var mappedResult = new List<PostCategoryIndexViewModel> { new PostCategoryIndexViewModel { Title = "Category1" } };
            _mapperMock.Setup(m => m.Map<IEnumerable<PostCategory>, IEnumerable<PostCategoryIndexViewModel>>(It.IsAny<IEnumerable<PostCategory>>()))
                .Returns(mappedResult);

            // Act
            var result = await _controller.LoadPostCategoriesTable(dtParameters);

            // Assert
            Assert.IsInstanceOf<JsonResult>(result);
            var jsonResult = (JsonResult)result;
            var data = (DatatableJsonResultModel)jsonResult.Value!;
            Assert.AreEqual(1, data.Draw);
            Assert.AreEqual(postCategories.Count(), data.RecordsTotal);
            Assert.AreEqual(1, data.RecordsFiltered);
            Assert.AreEqual(mappedResult, data.Data);
        }

        [Test]
        public async Task Details_ExistingPostCategory_ReturnsViewWithModel()
        {
            // Arrange
            var category = new PostCategory { Id = 1 };
            var viewModel = new PostCategoryFormViewModel { Id = 1 };

            _unitOfWorkMock.Setup(u => u.PostCategoryRepository.GetById(1)).ReturnsAsync(category);
            _mapperMock.Setup(m => m.Map<PostCategory, PostCategoryFormViewModel>(category)).Returns(viewModel);

            // Act
            var result = await _controller.Details(1);

            // Assert
            Assert.IsInstanceOf<ViewResult>(result);
            var viewResult = (ViewResult)result;
            Assert.AreEqual(viewModel, viewResult.Model);
        }

        [Test]
        public async Task Details_NonExistingPostCategory_ReturnsNotFound()
        {
            // Arrange
            _unitOfWorkMock.Setup(u => u.PostCategoryRepository.GetById(1)).ReturnsAsync((PostCategory)null);

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
        public async Task Create_Post_InvalidModelState_ReturnsViewWithModel()
        {
            // Arrange
            var model = new PostCategoryFormViewModel();
            _controller.ModelState.AddModelError("Error", "Invalid");

            // Act
            var result = await _controller.Create(model);

            // Assert
            Assert.IsInstanceOf<ViewResult>(result);
            var viewResult = (ViewResult)result;
            Assert.AreEqual(model, viewResult.Model);
        }

        [Test]
        public async Task Create_Post_DuplicateTitle_ReturnsViewWithError()
        {
            // Arrange
            var model = new PostCategoryFormViewModel { Title = "Duplicate" };
            var postCategory = new PostCategory();

            _mapperMock.Setup(m => m.Map<PostCategoryFormViewModel, PostCategory>(model)).Returns(postCategory);
            _unitOfWorkMock.Setup(u => u.PostCategoryRepository.IsPostCategoryExists(postCategory)).ReturnsAsync(true);

            // Act
            var result = await _controller.Create(model);

            // Assert
            Assert.IsInstanceOf<ViewResult>(result);
            Assert.IsTrue(_controller.ModelState.ContainsKey(nameof(PostCategoryFormViewModel.Title)));
        }

        [Test]
        public async Task Create_Post_ValidModel_InsertsAndRedirects()
        {
            // Arrange
            var model = new PostCategoryFormViewModel { Title = "NewCategory" };
            var postCategory = new PostCategory();

            _mapperMock.Setup(m => m.Map<PostCategoryFormViewModel, PostCategory>(model)).Returns(postCategory);
            _unitOfWorkMock.Setup(u => u.PostCategoryRepository.IsPostCategoryExists(postCategory)).ReturnsAsync(false);
            _unitOfWorkMock.Setup(u => u.PostCategoryRepository.Insert(postCategory));
            _unitOfWorkMock.Setup(u => u.Complete()).Returns(Task.CompletedTask);

            // Act
            var result = await _controller.Create(model);

            // Assert
            _unitOfWorkMock.Verify(u => u.PostCategoryRepository.Insert(postCategory), Times.Once);
            _unitOfWorkMock.Verify(u => u.Complete(), Times.Once);

            Assert.IsInstanceOf<RedirectToActionResult>(result);
            var redirect = (RedirectToActionResult)result;
            Assert.AreEqual(nameof(_controller.Index), redirect.ActionName);
        }

        [Test]
        public async Task Edit_Get_ExistingPostCategory_ReturnsViewWithModel()
        {
            // Arrange
            var category = new PostCategory { Id = 1 };
            var viewModel = new PostCategoryFormViewModel { Id = 1 };

            _unitOfWorkMock.Setup(u => u.PostCategoryRepository.GetById(1)).ReturnsAsync(category);
            _mapperMock.Setup(m => m.Map<PostCategory, PostCategoryFormViewModel>(category)).Returns(viewModel);

            // Act
            var result = await _controller.Edit(1);

            // Assert
            Assert.IsInstanceOf<ViewResult>(result);
            var viewResult = (ViewResult)result;
            Assert.AreEqual(viewModel, viewResult.Model);
        }

        [Test]
        public async Task Edit_Get_NonExistingPostCategory_ReturnsNotFound()
        {
            // Arrange
            _unitOfWorkMock.Setup(u => u.PostCategoryRepository.GetById(1)).ReturnsAsync((PostCategory)null);

            // Act
            var result = await _controller.Edit(1);
            
            // Assert
            Assert.IsInstanceOf<NotFoundResult>(result);
        }

        [Test]
        public async Task Edit_Post_IdMismatch_ReturnsBadRequest()
        {
            // Arrange
            var model = new PostCategoryFormViewModel { Id = 2 };

            // Act
            var result = await _controller.Edit(1, model);

            // Assert
            Assert.IsInstanceOf<BadRequestResult>(result);
        }

        [Test]
        public async Task Edit_Post_InvalidModelState_ReturnsViewWithModel()
        {
            // Arrange
            var model = new PostCategoryFormViewModel { Id = 1 };
            _controller.ModelState.AddModelError("Error", "Invalid");

            // Act
            var result = await _controller.Edit(1, model);

            // Assert
            Assert.IsInstanceOf<ViewResult>(result);
            var viewResult = (ViewResult)result;
            Assert.AreEqual(model, viewResult.Model);
        }

        [Test]
        public async Task Edit_Post_DuplicateTitle_ReturnsViewWithError()
        {
            // Arrange
            var model = new PostCategoryFormViewModel { Id = 1, Title = "Duplicate" };
            var postCategory = new PostCategory();

            _mapperMock.Setup(m => m.Map<PostCategoryFormViewModel, PostCategory>(model)).Returns(postCategory);
            _unitOfWorkMock.Setup(u => u.PostCategoryRepository.IsPostCategoryExists(postCategory)).ReturnsAsync(true);

            // Act
            var result = await _controller.Edit(1, model);

            // Assert
            Assert.IsInstanceOf<ViewResult>(result);
            Assert.IsTrue(_controller.ModelState.ContainsKey(nameof(PostCategoryFormViewModel.Title)));
        }

        [Test]
        public async Task Edit_Post_ValidModel_UpdatesAndRedirects()
        {
            // Arrange
            var model = new PostCategoryFormViewModel { Id = 1, Title = "Valid" };
            var postCategory = new PostCategory();

            _mapperMock.Setup(m => m.Map<PostCategoryFormViewModel, PostCategory>(model)).Returns(postCategory);
            _unitOfWorkMock.Setup(u => u.PostCategoryRepository.IsPostCategoryExists(postCategory)).ReturnsAsync(false);
            _unitOfWorkMock.Setup(u => u.PostCategoryRepository.Update(postCategory));
            _unitOfWorkMock.Setup(u => u.Complete()).Returns(Task.CompletedTask);

            // Act
            var result = await _controller.Edit(1, model);

            // Assert
            _unitOfWorkMock.Verify(u => u.PostCategoryRepository.Update(postCategory), Times.Once);
            _unitOfWorkMock.Verify(u => u.Complete(), Times.Once);

            Assert.IsInstanceOf<RedirectToActionResult>(result);
            var redirect = (RedirectToActionResult)result;
            Assert.AreEqual(nameof(_controller.Index), redirect.ActionName);
        }

        [Test]
        public async Task Delete_Get_ExistingPostCategory_ReturnsViewWithModel()
        {
            // Arrange
            var category = new PostCategory { Id = 1 };
            var viewModel = new PostCategoryFormViewModel { Id = 1 };

            _unitOfWorkMock.Setup(u => u.PostCategoryRepository.GetById(1)).ReturnsAsync(category);
            _mapperMock.Setup(m => m.Map<PostCategory, PostCategoryFormViewModel>(category)).Returns(viewModel);

            // Act
            var result = await _controller.Delete(1);

            // Assert
            Assert.IsInstanceOf<ViewResult>(result);
            var viewResult = (ViewResult)result;
            Assert.AreEqual(viewModel, viewResult.Model);
        }

        [Test]
        public async Task Delete_Get_NonExistingPostCategory_ReturnsNotFound()
        {
            // Arrange
            _unitOfWorkMock.Setup(u => u.PostCategoryRepository.GetById(1)).ReturnsAsync((PostCategory)null);

            // Act
            var result = await _controller.Delete(1);

            // Assert
            Assert.IsInstanceOf<NotFoundResult>(result);
        }

        [Test]
        public async Task DeleteConfirmed_Post_DeletesAndRedirects()
        {
            // Arrange
            var category = new PostCategory { Id = 1 };

            _unitOfWorkMock.Setup(u => u.PostCategoryRepository.GetById(1)).ReturnsAsync(category);
            _unitOfWorkMock.Setup(u => u.PostCategoryRepository.Delete(category.Id));
            _unitOfWorkMock.Setup(u => u.Complete()).Returns(Task.CompletedTask);

            // Act
            var result = await _controller.DeleteConfirmed(1);

            // Assert
            _unitOfWorkMock.Verify(u => u.PostCategoryRepository.Delete(category.Id), Times.Once);
            _unitOfWorkMock.Verify(u => u.Complete(), Times.Once);

            Assert.IsInstanceOf<RedirectToActionResult>(result);
            var redirect = (RedirectToActionResult)result;
            Assert.AreEqual(nameof(_controller.Index), redirect.ActionName);
        }
    }
}
