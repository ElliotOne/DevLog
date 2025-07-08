using System.Security.Claims;
using System.Text;
using AutoMapper;
using DevLog.Areas.Panel.Controllers;
using DevLog.Areas.Panel.Models;
using DevLog.Areas.Panel.Models.ViewModels;
using DevLog.Core.Domain;
using DevLog.Models.Constants;
using DevLog.Persistence;
using DevLog.Persistence.Repositories;
using DevLog.Services.FileHandler;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.Primitives;
using Moq;

namespace DevLog.UnitTests.Areas.Panel.Controllers
{
    [TestFixture]
    public class PostsControllerTests
    {
        private Mock<IUnitOfWork> _unitOfWorkMock;
        private Mock<IUserRepository> _userRepoMock;
        private Mock<IMapper> _mapperMock;
        private Mock<IFileHandler> _fileHandlerMock;
        private PostsController _controller;
        private ClaimsPrincipal _userPrincipal;

        [SetUp]
        public void SetUp()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _userRepoMock = new Mock<IUserRepository>();
            _mapperMock = new Mock<IMapper>();
            _fileHandlerMock = new Mock<IFileHandler>();

            _controller = new PostsController(
                _unitOfWorkMock.Object,
                _mapperMock.Object,
                _fileHandlerMock.Object
            );

            _unitOfWorkMock.SetupGet(u => u.UserRepository).Returns(_userRepoMock.Object);

            _userPrincipal = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, "123"),
                new Claim(ClaimTypes.Name, "testuser"),
            }, "mock"));

            var httpContext = new DefaultHttpContext
            {
                User = _userPrincipal
            };

            // Setup mock file
            var fileContent = "fake content";
            var fileName = "test.jpg";
            var stream = new MemoryStream(Encoding.UTF8.GetBytes(fileContent));
            var formFile = new FormFile(stream, 0, stream.Length, "file", fileName)
            {
                Headers = new HeaderDictionary(),
                ContentType = "image/jpeg"
            };

            var files = new FormFileCollection { formFile };

            // Add Form feature with file to HttpContext
            var formCollection = new FormCollection(new Dictionary<string, StringValues>(), files);
            httpContext.Features.Set<IFormFeature>(new FormFeature(formCollection));

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            _controller.TempData = new TempDataDictionary(
                httpContext,
                Mock.Of<ITempDataProvider>());
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
        public async Task LoadPostsTable_WithSearchAndOrdering_ReturnsJsonResult()
        {
            // Arrange
            var posts = new List<Post>
            {
                new Post { Id = 1, Title = "Post1", CreateDate = DateTime.Now, LastEditDate = DateTime.Now, UserId = 123},
                new Post { Id = 2, Title = "Post2", CreateDate = DateTime.Now, LastEditDate = DateTime.Now, UserId = 124}
            }.AsQueryable();

            var dtParameters = new DTParameters
            {
                Draw = 1,
                Start = 0,
                Length = 10,
                Search = new DTSearch { Value = "Post1" },
                Order = new DTOrder[] { new DTOrder { Column = 0, Dir = DTOrderDir.ASC } },
                Columns = new DTColumn[] { new DTColumn { Data = nameof(Post.Title) } }
            };

            var user = new User { Id = 123 };

            _unitOfWorkMock.Setup(u => u.PostRepository.GetAll()).Returns(posts);
            _unitOfWorkMock.Setup(u => u.PostRepository.Count()).ReturnsAsync(posts.Count());
            _userRepoMock.Setup(r => r.GetByClaimsPrincipal(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(user);
            _userRepoMock.Setup(r => r.IsInRole(user, UserRolesConstant.SuperAdmin)).ReturnsAsync(false);

            var mappedResult = new List<PostIndexViewModel> { new PostIndexViewModel { Title = "Category1" } };
            _mapperMock.Setup(m => m.Map<IEnumerable<Post>, IEnumerable<PostIndexViewModel>>(It.IsAny<IEnumerable<Post>>()))
                .Returns(mappedResult);

            // Act
            var result = await _controller.LoadPostsTable(dtParameters);

            // Assert
            Assert.IsInstanceOf<JsonResult>(result);
            var jsonResult = (JsonResult)result;
            var data = (DatatableJsonResultModel)jsonResult.Value!;
            Assert.AreEqual(1, data.Draw);
            Assert.AreEqual(posts.Count(), data.RecordsTotal);
            Assert.AreEqual(1, data.RecordsFiltered);
            Assert.AreEqual(mappedResult, data.Data);
        }

        [Test]
        public async Task Details_InvalidId_ReturnsNotFound()
        {
            // Arrange
            _unitOfWorkMock.Setup(u => u.PostRepository.GetById(It.IsAny<int>())).ReturnsAsync((Post)null);

            // Act
            var result = await _controller.Details(1);

            // Assert
            Assert.IsInstanceOf<NotFoundResult>(result);
        }

        [Test]
        public async Task Create_PostWithInvalidModel_ReturnsViewWithModel()
        {
            // Arrange
            _unitOfWorkMock.Setup(u => u.PostCategoryRepository.GetAll())
                .Returns(new List<PostCategory>
                {
                    new PostCategory { Id = 1, Title = "Category 1" },
                    new PostCategory { Id = 2, Title = "Category 2" }
                }.AsQueryable());

            _controller.ModelState.AddModelError("Title", "Required");

            var viewModel = new PostFormViewModel();

            // Act
            var result = await _controller.Create(viewModel) as ViewResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOf<PostFormViewModel>(result.Model);
            var returnedModel = result.Model as PostFormViewModel;

            Assert.IsNotNull(returnedModel);
            Assert.AreEqual(viewModel.Title, returnedModel.Title);

            // Check ViewData (not ViewBag) because your controller uses ViewData
            var postCategories = _controller.ViewData[nameof(PostFormViewModel.PostCategoryId)] as SelectList;

            Assert.IsNotNull(postCategories);
            Assert.AreEqual(2, postCategories.Count());
            // Optional: check the items
            var titles = postCategories.Select(pc => (pc.Value).ToString()).ToList();
        }

        [Test]
        public async Task Create_ValidPost_RedirectsToIndex()
        {
            // Arrange
            var user = new User { Id = 1 };
            _unitOfWorkMock.Setup(u => u.UserRepository.GetByClaimsPrincipal(It.IsAny<ClaimsPrincipal>()))
                .ReturnsAsync(user);

            _unitOfWorkMock.Setup(u => u.PostCategoryRepository.GetAll())
                .Returns(new List<PostCategory> { new PostCategory { Id = 1, Title = "Category 1" } }.AsQueryable());

            var fileContent = "dummy file";
            var stream = new MemoryStream(Encoding.UTF8.GetBytes(fileContent));
            var formFile = new FormFile(stream, 0, stream.Length, "CoverImage", "test.jpg")
            {
                Headers = new HeaderDictionary(),
                ContentType = "image/jpeg"
            };

            var viewModel = new PostFormViewModel
            {
                Title = "Test Post",
                File = formFile
            };

            var domainModel = new Post { Title = "Test Post" };
            _mapperMock.Setup(m => m.Map<PostFormViewModel, Post>(viewModel)).Returns(domainModel);
            _fileHandlerMock.Setup(f => f.GetRelativePath(It.IsAny<string>(), It.IsAny<string>(), FileType.Post))
                .Returns("fakepath.jpg");
            _fileHandlerMock.Setup(f => f.Upload(formFile, "fakepath.jpg")).ReturnsAsync(true);

            var httpContext = new DefaultHttpContext
            {
                User = _userPrincipal
            };
            _controller.ControllerContext = new ControllerContext { HttpContext = httpContext };
            _controller.TempData = new TempDataDictionary(httpContext, Mock.Of<ITempDataProvider>());

            _unitOfWorkMock.Setup(u => u.PostRepository.Insert(It.IsAny<Post>()));

            // Act
            var result = await _controller.Create(viewModel);

            // Assert
            _unitOfWorkMock.Verify(u => u.PostRepository.Insert(It.IsAny<Post>()), Times.Once);
            _unitOfWorkMock.Verify(u => u.Complete(), Times.Once);
            Assert.IsInstanceOf<RedirectToActionResult>(result);
            Assert.AreEqual("Index", ((RedirectToActionResult)result).ActionName);
        }

        [Test]
        public async Task Edit_Get_InvalidId_ReturnsNotFound()
        {
            // Arrange
            _unitOfWorkMock.Setup(u => u.PostRepository.GetById(It.IsAny<int>()))
                .ReturnsAsync((Post)null);

            // Act
            var result = await _controller.Edit(1);

            // Assert
            Assert.IsInstanceOf<NotFoundResult>(result);
        }

        [Test]
        public async Task Edit_Post_MismatchedIds_ReturnsBadRequest()
        {
            // Arrange
            var viewModel = new PostFormViewModel { Id = 2 };

            // Act
            var result = await _controller.Edit(1, viewModel);

            // Assert
            Assert.IsInstanceOf<BadRequestResult>(result);
        }

        [Test]
        public async Task Delete_Get_InvalidId_ReturnsNotFound()
        {
            // Arrange
            _unitOfWorkMock.Setup(u => u.PostRepository.GetById(It.IsAny<int>()))
                .ReturnsAsync((Post)null);

            // Act
            var result = await _controller.Delete(1);

            // Assert
            Assert.IsInstanceOf<NotFoundResult>(result);
        }

        [Test]
        public async Task DeleteConfirmed_InvalidImagePath_ReturnsBadRequest()
        {
            // Arrange
            var user = new User { Id = 1 };

            _unitOfWorkMock.Setup(u => u.UserRepository.GetByClaimsPrincipal(It.IsAny<ClaimsPrincipal>()))
                .ReturnsAsync(user);

            _unitOfWorkMock.Setup(u => u.UserRepository
                    .IsUserAllowedForOperation(user, 1, UserRolesConstant.SuperAdmin))
                .ReturnsAsync(true);

            // Act
            var result = await _controller.DeleteConfirmed(1, 1, null);

            // Assert
            Assert.IsInstanceOf<BadRequestResult>(result);
        }

        [Test]
        public async Task DeleteConfirmed_ValidCall_DeletesAndRedirects()
        {
            // Arrange
            var user = new User { Id = 1 };

            _unitOfWorkMock.Setup(u => u.UserRepository.GetByClaimsPrincipal(It.IsAny<ClaimsPrincipal>()))
                .ReturnsAsync(user);

            _unitOfWorkMock.Setup(u => u.UserRepository
                    .IsUserAllowedForOperation(user, 1, UserRolesConstant.SuperAdmin))
                .ReturnsAsync(true);

            _unitOfWorkMock.Setup(u => u.PostRepository.Delete(It.IsAny<int>())).Returns(Task.CompletedTask);

            // Act
            var result = await _controller.DeleteConfirmed(1, 1, "path/to/image");

            // Assert
            _fileHandlerMock.Verify(f => f.Delete("path/to/image"), Times.Once);
            _unitOfWorkMock.Verify(u => u.PostRepository.Delete(1), Times.Once);
            _unitOfWorkMock.Verify(u => u.Complete(), Times.Once);
            Assert.IsInstanceOf<RedirectToActionResult>(result);
        }
    }
}
