using AutoMapper;
using DevLog.Controllers;
using DevLog.Core.Domain;
using DevLog.Infrastructure;
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
    public class BlogControllerTests
    {
        private Mock<IUnitOfWork> _unitOfWorkMock;
        private Mock<IMapper> _mapperMock;
        private Mock<IHttpContextAccessor> _httpContextAccessorMock;
        private BlogController _controller;

        [SetUp]
        public void Setup()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _mapperMock = new Mock<IMapper>();
            _httpContextAccessorMock = new Mock<IHttpContextAccessor>();
            _controller = new BlogController(
                _unitOfWorkMock.Object,
                _mapperMock.Object,
                _httpContextAccessorMock.Object);
        }

        [Test]
        public void Index_ReturnsViewWithExpectedModel()
        {
            // Arrange
            var posts = new List<Post> { new Post { Id = 1, Title = "Test Post" } };
            var pagedPosts = new PagedList<Post>(posts.AsQueryable(), 0, 6);

            _unitOfWorkMock.Setup(u => u.PostRepository.GetAll())
                .Returns(posts.AsQueryable());

            _mapperMock.Setup(m => m.Map<IEnumerable<Post>, IEnumerable<PostViewModel>>(It.IsAny<IEnumerable<Post>>()))
                .Returns(new List<PostViewModel> { new PostViewModel { Id = 1, Title = "Test Post" } });

            // Act
            var result = _controller.Index(1, null, null, null);

            // Assert
            Assert.IsInstanceOf<ViewResult>(result);
            var viewModel = (result as ViewResult)?.Model as PostsViewModel;
            Assert.NotNull(viewModel);
            Assert.AreEqual(1, viewModel.PostViewModels.Count());
        }

        [Test]
        public void Index_WithSearchStringAndFilters_ReturnsFilteredPosts()
        {
            // Arrange
            var posts = new List<Post>
            {
                new Post { Id = 1, Title = "Test Post", PostCategoryId = 2, Tags = "tag1,tag2" },
                new Post { Id = 2, Title = "Another Post", PostCategoryId = 3, Tags = "tag3" }
            };
            var pagedPosts = new PagedList<Post>(posts.AsQueryable(), 0, 6);

            _unitOfWorkMock.Setup(u => u.PostRepository.GetAll())
                .Returns(posts.AsQueryable());

            _mapperMock.Setup(m => m.Map<IEnumerable<Post>, IEnumerable<PostViewModel>>(It.IsAny<IEnumerable<Post>>()))
                .Returns(posts.Select(p => new PostViewModel { Id = p.Id, Title = p.Title }).ToList());

            // Act
            var result = _controller.Index(1, "Test", 2, "tag1");

            // Assert
            Assert.IsInstanceOf<ViewResult>(result);
            var viewModel = (result as ViewResult)?.Model as PostsViewModel;
            Assert.NotNull(viewModel);
            Assert.IsTrue(viewModel.PostViewModels.Any(p => p.Title.Contains("Test")));
        }

        [Test]
        public async Task Post_PostNotFound_ReturnsNotFound()
        {
            // Arrange
            _unitOfWorkMock.Setup(u => u.PostRepository.GetById(It.IsAny<int>()))
                .ReturnsAsync((Post)null);

            // Act
            var result = await _controller.Post(1);

            // Assert
            Assert.IsInstanceOf<NotFoundResult>(result);
        }

        [Test]
        public async Task Post_PostFound_ReturnsViewWithPostViewModel()
        {
            // Arrange
            var post = new Post { Id = 1, Title = "Test" };
            var postViewModel = new PostViewModel { Id = 1, Title = "Test" };

            _unitOfWorkMock.Setup(u => u.PostRepository.GetById(1))
                .ReturnsAsync(post);

            _unitOfWorkMock.Setup(u => u.PostCommentRepository.GetAllByPostId(1))
                .ReturnsAsync(new List<PostComment>());

            _mapperMock.Setup(m => m.Map<Post, PostViewModel>(post)).Returns(postViewModel);
            _mapperMock.Setup(m => m.Map<IEnumerable<PostComment>, IEnumerable<PostCommentViewModel>>(It.IsAny<IEnumerable<PostComment>>()))
                .Returns(new List<PostCommentViewModel>());

            // Act
            var result = await _controller.Post(1);

            // Assert
            Assert.IsInstanceOf<ViewResult>(result);
            var model = (result as ViewResult)?.Model as PostViewModel;
            Assert.NotNull(model);
            Assert.AreEqual(1, model.Id);
        }

        [Test]
        public async Task Comment_Post_InvalidModelState_ReturnsJsonModelStateInvalid()
        {
            // Arrange
            _controller.ModelState.AddModelError("Error", "Model is invalid");

            // Act
            var result = await _controller.CommentPost(new PostCommentFormViewModel());

            // Assert
            Assert.IsInstanceOf<JsonResult>(result);
            var json = result as JsonResult;
            var data = json.Value as JsonResultModel;

            Assert.AreEqual(JsonResultStatusCode.ModelStateIsNotValid, data.StatusCode);
        }

        [Test]
        public async Task Comment_Post_ValidModelState_ReturnsSuccessJson()
        {
            // Arrange
            var model = new PostCommentFormViewModel();
            var comment = new PostComment();

            _mapperMock.Setup(m => m.Map<PostCommentFormViewModel, PostComment>(model)).Returns(comment);

            var mockContext = new DefaultHttpContext();
            mockContext.Connection.RemoteIpAddress = System.Net.IPAddress.Parse("127.0.0.1");
            _httpContextAccessorMock.Setup(a => a.HttpContext).Returns(mockContext);

            _unitOfWorkMock.Setup(u => u.PostCommentRepository.Insert(comment));
            _unitOfWorkMock.Setup(u => u.Complete()).Returns(Task.CompletedTask);

            // Act
            var result = await _controller.CommentPost(model);

            // Assert
            Assert.IsInstanceOf<JsonResult>(result);
            var json = result as JsonResult;
            var data = json.Value as JsonResultModel;

            Assert.AreEqual(JsonResultStatusCode.Success, data.StatusCode);
            Assert.AreEqual(MessagesConstant.CommentSentAndWillShowAfterAcceptance, data.Message);
        }
    }
}
