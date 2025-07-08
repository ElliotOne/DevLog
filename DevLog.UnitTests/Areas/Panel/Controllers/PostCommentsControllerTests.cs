using System.Net;
using System.Security.Claims;
using AutoMapper;
using DevLog.Areas.Panel.Controllers;
using DevLog.Areas.Panel.Models;
using DevLog.Areas.Panel.Models.ViewModels;
using DevLog.Core.Domain;
using DevLog.Models.Constants;
using DevLog.Persistence;
using DevLog.Persistence.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using PostCommentStatus = DevLog.Core.Domain.PostCommentStatus;

namespace DevLog.UnitTests.Areas.Panel.Controllers
{
    [TestFixture]
    public class PostCommentsControllerTests
    {
        private Mock<IUnitOfWork> _unitOfWorkMock;
        private Mock<IMapper> _mapperMock;
        private Mock<IHttpContextAccessor> _httpContextAccessorMock;
        private PostCommentsController _controller;
        private Mock<IUserRepository> _userRepoMock;
        private Mock<IPostCommentRepository> _postCommentRepoMock;
        private ClaimsPrincipal _userPrincipal;

        [SetUp]
        public void Setup()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _mapperMock = new Mock<IMapper>();
            _httpContextAccessorMock = new Mock<IHttpContextAccessor>();

            _userRepoMock = new Mock<IUserRepository>();
            _postCommentRepoMock = new Mock<IPostCommentRepository>();

            _unitOfWorkMock.SetupGet(u => u.UserRepository).Returns(_userRepoMock.Object);
            _unitOfWorkMock.SetupGet(u => u.PostCommentRepository).Returns(_postCommentRepoMock.Object);

            _userPrincipal = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, "123"),
                new Claim(ClaimTypes.Name, "testuser"),
            }, "mock"));

            var httpContextMock = new DefaultHttpContext { User = _userPrincipal };
            _httpContextAccessorMock.Setup(a => a.HttpContext).Returns(httpContextMock);
            _controller = new PostCommentsController(_unitOfWorkMock.Object, _mapperMock.Object, _httpContextAccessorMock.Object);
            _controller.ControllerContext.HttpContext = httpContextMock;
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
        public async Task LoadPostCommentsTable_DefaultOrderingAndFiltering_ReturnsJsonResult()
        {
            // Arrange
            var dtParameters = new DTParameters
            {
                Draw = 1,
                Start = 0,
                Length = 10,
                Search = new DTSearch { Value = "search" },
                Order = null,
                Columns = new DTColumn[] { new DTColumn { Data = nameof(PostComment.Id) } }
            };

            var postComments = new List<PostComment>
            {
                new PostComment { Id = 1, UserFullName = "John Doe", Body = "Body", Email = "a@b.com", Ip = "127.0.0.1", CreateDate = System.DateTime.Now, LastEditDate = System.DateTime.Now, Post = new Post { Title = "Test Post", UserId = 123 }, UserId = 10 },
                new PostComment { Id = 2, UserFullName = "Jane Smith", Body = "Other", Email = "c@d.com", Ip = "127.0.0.2", CreateDate = System.DateTime.Now, LastEditDate = System.DateTime.Now, Post = new Post { Title = "Another Post", UserId = 124 }, UserId = 11 }
            }.AsQueryable();

            var user = new User { Id = 123 };

            _postCommentRepoMock.Setup(r => r.GetAll()).Returns(postComments);
            _userRepoMock.Setup(r => r.GetByClaimsPrincipal(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(user);
            _userRepoMock.Setup(r => r.IsInRole(user, UserRolesConstant.SuperAdmin)).ReturnsAsync(false);
            _postCommentRepoMock.Setup(r => r.Count()).ReturnsAsync(postComments.Count());
            _mapperMock.Setup(m => m.Map<IEnumerable<PostComment>, IEnumerable<PostCommentIndexViewModel>>(It.IsAny<IEnumerable<PostComment>>()))
                .Returns(new List<PostCommentIndexViewModel> { new PostCommentIndexViewModel { Id = 1, UserFullName = "John Doe" } });

            // Act
            var result = await _controller.LoadPostCommentsTable(dtParameters);

            // Assert
            Assert.IsInstanceOf<JsonResult>(result);
            var jsonResult = (JsonResult)result;
            var data = jsonResult.Value as DatatableJsonResultModel;
            Assert.NotNull(data);
            Assert.AreEqual(1, data.Draw);
            Assert.AreEqual(postComments.Count(), data.RecordsTotal);
            Assert.GreaterOrEqual(data.RecordsFiltered, 0);
            Assert.IsNotNull(data.Data);
        }

        [Test]
        public async Task Details_ValidId_ReturnsView()
        {
            // Arrange
            var postComment = new PostComment { Id = 1, UserId = 10 };
            var user = new User { Id = 123 };

            _postCommentRepoMock.Setup(r => r.GetById(1)).ReturnsAsync(postComment);
            _userRepoMock.Setup(r => r.GetByClaimsPrincipal(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(user);
            _userRepoMock.Setup(r => r.IsUserAllowedForOperation(user, postComment.UserId, UserRolesConstant.SuperAdmin)).ReturnsAsync(true);
            _mapperMock.Setup(m => m.Map<PostComment, PostCommentFormViewModel>(postComment))
                .Returns(new PostCommentFormViewModel { Id = 1 });

            // Act
            var result = await _controller.Details(1);

            // Assert
            Assert.IsInstanceOf<ViewResult>(result);
            var viewResult = (ViewResult)result;
            Assert.IsInstanceOf<PostCommentFormViewModel>(viewResult.Model);
            Assert.AreEqual(1, ((PostCommentFormViewModel)viewResult.Model).Id);
        }

        [Test]
        public async Task Details_InvalidId_ReturnsNotFound()
        {
            // Arrange
            _postCommentRepoMock.Setup(r => r.GetById(5)).ReturnsAsync((PostComment?)null);
            
            // Act
            var result = await _controller.Details(5);

            // Assert
            Assert.IsInstanceOf<NotFoundResult>(result);
        }

        [Test]
        public async Task Details_UserNotAuthorized_ReturnsUnauthorized()
        {
            // Arrange
            var postComment = new PostComment { Id = 1, UserId = 10 };
            var user = new User { Id = 123 };

            _postCommentRepoMock.Setup(r => r.GetById(1)).ReturnsAsync(postComment);
            _userRepoMock.Setup(r => r.GetByClaimsPrincipal(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(user);
            _userRepoMock.Setup(r => r.IsUserAllowedForOperation(user, postComment.UserId, UserRolesConstant.SuperAdmin)).ReturnsAsync(false);

            // Act
            var result = await _controller.Details(1);

            // Assert
            Assert.IsInstanceOf<UnauthorizedResult>(result);
        }

        [Test]
        public async Task Reply_Get_ValidId_ReturnsView()
        {
            // Arrange
            var parentComment = new PostComment { Id = 1, UserId = 10 };
            var user = new User { Id = 123 };

            _postCommentRepoMock.Setup(r => r.GetById(1)).ReturnsAsync(parentComment);
            _userRepoMock.Setup(r => r.GetByClaimsPrincipal(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(user);
            _userRepoMock.Setup(r => r.IsUserAllowedForOperation(user, parentComment.UserId, UserRolesConstant.SuperAdmin)).ReturnsAsync(true);
            _mapperMock.Setup(m => m.Map<PostComment, PostCommentReplyFormViewModel>(parentComment))
                .Returns(new PostCommentReplyFormViewModel { Id = 1, ParentId = 1 });

            // Act
            var result = await _controller.Reply(1);

            // Assert
            Assert.IsInstanceOf<ViewResult>(result);
            var viewResult = (ViewResult)result;
            Assert.IsInstanceOf<PostCommentReplyFormViewModel>(viewResult.Model);
        }

        [Test]
        public async Task Reply_Get_InvalidId_ReturnsNotFound()
        {
            // Arrange
            _postCommentRepoMock.Setup(r => r.GetById(99)).ReturnsAsync((PostComment?)null);

            // Act
            var result = await _controller.Reply(99);

            // Assert
            Assert.IsInstanceOf<NotFoundResult>(result);
        }

        [Test]
        public async Task Reply_Get_UserNotAuthorized_ReturnsUnauthorized()
        {
            // Arrange
            var parentComment = new PostComment { Id = 1, UserId = 10 };
            var user = new User { Id = 123 };

            _postCommentRepoMock.Setup(r => r.GetById(1)).ReturnsAsync(parentComment);
            _userRepoMock.Setup(r => r.GetByClaimsPrincipal(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(user);
            _userRepoMock.Setup(r => r.IsUserAllowedForOperation(user, parentComment.UserId, UserRolesConstant.SuperAdmin)).ReturnsAsync(false);

            // Act
            var result = await _controller.Reply(1);

            // Assert
            Assert.IsInstanceOf<UnauthorizedResult>(result);
        }

        [Test]
        public async Task Reply_Post_IdMismatch_ReturnsBadRequest()
        {
            // Arrange
            var vm = new PostCommentReplyFormViewModel { Id = 2, ParentId = 1 };
            
            // Act
            var result = await _controller.Reply(1, vm);

            // Assert
            Assert.IsInstanceOf<BadRequestResult>(result);
        }

        [Test]
        public async Task Reply_Post_ModelStateInvalid_ReturnsView()
        {
            // Arrange
            var vm = new PostCommentReplyFormViewModel { Id = 1, ParentId = 1 };
            _controller.ModelState.AddModelError("Test", "Error");

            // Act
            var result = await _controller.Reply(1, vm);

            // Assert
            Assert.IsInstanceOf<ViewResult>(result);
            var viewResult = (ViewResult)result;
            Assert.AreEqual(vm, viewResult.Model);
        }

        [Test]
        public async Task Reply_Post_ParentNotFound_ReturnsNotFound()
        {
            // Arrange
            var vm = new PostCommentReplyFormViewModel { Id = 1, ParentId = 99 };

            _postCommentRepoMock.Setup(r => r.GetById(vm.ParentId)).ReturnsAsync((PostComment?)null);

            // Act
            var result = await _controller.Reply(1, vm);

            // Assert
            Assert.IsInstanceOf<NotFoundResult>(result);
        }

        [Test]
        public async Task Reply_Post_UserNotAuthorized_ReturnsUnauthorized()
        {
            // Arrange
            var parentComment = new PostComment { Id = 1, UserId = 10 };
            var vm = new PostCommentReplyFormViewModel { Id = 1, ParentId = 1 };
            var user = new User { Id = 123 };

            _postCommentRepoMock.Setup(r => r.GetById(vm.ParentId)).ReturnsAsync(parentComment);
            _userRepoMock.Setup(r => r.GetByClaimsPrincipal(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(user);
            _userRepoMock.Setup(r => r.IsUserAllowedForOperation(user, parentComment.UserId, UserRolesConstant.SuperAdmin)).ReturnsAsync(false);

            // Act
            var result = await _controller.Reply(1, vm);

            // Assert
            Assert.IsInstanceOf<UnauthorizedResult>(result);
        }

        [Test]
        public async Task Reply_Post_Valid_InsertsAndRedirects()
        {
            // Arrange
            var parentComment = new PostComment { Id = 1, UserId = 10 };
            var vm = new PostCommentReplyFormViewModel { Id = 1, ParentId = 1 };
            var user = new User { Id = 123 };

            _postCommentRepoMock.Setup(r => r.GetById(vm.ParentId)).ReturnsAsync(parentComment);
            _userRepoMock.Setup(r => r.GetByClaimsPrincipal(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(user);
            _userRepoMock.Setup(r => r.IsUserAllowedForOperation(user, parentComment.UserId, UserRolesConstant.SuperAdmin)).ReturnsAsync(true);

            var postComment = new PostComment();
            _mapperMock.Setup(m => m.Map<PostCommentReplyFormViewModel, PostComment>(vm)).Returns(postComment);

            var remoteIp = IPAddress.Parse("192.168.0.1");
            _httpContextAccessorMock.Setup(a => a.HttpContext.Connection.RemoteIpAddress).Returns(remoteIp);

            _postCommentRepoMock.Setup(r => r.Insert(postComment));
            _unitOfWorkMock.Setup(u => u.Complete()).Returns(Task.CompletedTask);

            // Act
            var result = await _controller.Reply(vm.Id, vm);

            // Assert
            Assert.IsInstanceOf<RedirectToActionResult>(result);
            var redirect = (RedirectToActionResult)result;
            Assert.AreEqual(nameof(PostCommentsController.Index), redirect.ActionName);

            Assert.AreEqual(user.Id, postComment.UserId);
            Assert.AreEqual(remoteIp.ToString(), postComment.Ip);
            _postCommentRepoMock.Verify(r => r.Insert(postComment), Times.Once);
            _unitOfWorkMock.Verify(u => u.Complete(), Times.Once);
        }

        [Test]
        public async Task Edit_Get_ValidId_ReturnsView()
        {
            // Arrange
            var postComment = new PostComment { Id = 1, UserId = 10 };
            var user = new User { Id = 123 };

            _postCommentRepoMock.Setup(r => r.GetById(1)).ReturnsAsync(postComment);
            _userRepoMock.Setup(r => r.GetByClaimsPrincipal(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(user);
            _userRepoMock.Setup(r => r.IsUserAllowedForOperation(user, postComment.UserId, UserRolesConstant.SuperAdmin)).ReturnsAsync(true);
            _mapperMock.Setup(m => m.Map<PostComment, PostCommentFormViewModel>(postComment))
                .Returns(new PostCommentFormViewModel { Id = 1 });

            // Act
            var result = await _controller.Edit(1);

            // Assert
            Assert.IsInstanceOf<ViewResult>(result);
            var viewResult = (ViewResult)result;
            Assert.IsInstanceOf<PostCommentFormViewModel>(viewResult.Model);
        }

        [Test]
        public async Task Edit_Get_InvalidId_ReturnsNotFound()
        {
            // Arrange
            _postCommentRepoMock.Setup(r => r.GetById(5)).ReturnsAsync((PostComment?)null);

            // Act
            var result = await _controller.Edit(5);

            // Assert
            Assert.IsInstanceOf<NotFoundResult>(result);
        }

        [Test]
        public async Task Edit_Get_UserNotAuthorized_ReturnsUnauthorized()
        {
            // Arrange
            var postComment = new PostComment { Id = 1, UserId = 10 };
            var user = new User { Id = 123 };

            _postCommentRepoMock.Setup(r => r.GetById(1)).ReturnsAsync(postComment);
            _userRepoMock.Setup(r => r.GetByClaimsPrincipal(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(user);
            _userRepoMock.Setup(r => r.IsUserAllowedForOperation(user, postComment.UserId, UserRolesConstant.SuperAdmin)).ReturnsAsync(false);

            // Act
            var result = await _controller.Edit(1);

            // Assert
            Assert.IsInstanceOf<UnauthorizedResult>(result);
        }

        [Test]
        public async Task Edit_Post_IdMismatch_ReturnsBadRequest()
        {
            // Arrange
            var vm = new PostCommentFormViewModel { Id = 2 };

            // Act
            var result = await _controller.Edit(1, vm);

            // Assert
            Assert.IsInstanceOf<BadRequestResult>(result);
        }

        [Test]
        public async Task Edit_Post_ModelStateInvalid_ReturnsView()
        {
            // Arrange
            _controller.ModelState.AddModelError("Test", "Error");
            var vm = new PostCommentFormViewModel { Id = 1 };

            // Act
            var result = await _controller.Edit(1, vm);

            // Assert
            Assert.IsInstanceOf<ViewResult>(result);
            var viewResult = (ViewResult)result;
            Assert.AreEqual(vm, viewResult.Model);
        }

        [Test]
        public async Task Edit_Post_UserNotAuthorized_ReturnsUnauthorized()
        {
            // Arrange
            var postComment = new PostComment { Id = 1, UserId = 10 };
            var vm = new PostCommentFormViewModel { Id = 1 };
            var user = new User { Id = 123 };

            _postCommentRepoMock.Setup(r => r.GetById(1)).ReturnsAsync(postComment);
            _userRepoMock.Setup(r => r.GetByClaimsPrincipal(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(user);
            _userRepoMock.Setup(r => r.IsUserAllowedForOperation(user, postComment.UserId, UserRolesConstant.SuperAdmin)).ReturnsAsync(false);

            // Act
            var result = await _controller.Edit(1, vm);

            // Assert
            Assert.IsInstanceOf<UnauthorizedResult>(result);
        }

        [Test]
        public async Task Edit_Post_Valid_UpdatesAndRedirects()
        {
            // Arrange
            var postComment = new PostComment { Id = 1, UserId = 10, Status = PostCommentStatus.Accepted, Children = new List<PostComment>() };
            var vm = new PostCommentFormViewModel { Id = 1, PostUserId = 10 };
            var user = new User { Id = 123 };

            _userRepoMock.Setup(r => r.GetByClaimsPrincipal(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(user);
            _userRepoMock.Setup(r => r.IsUserAllowedForOperation(user, vm.PostUserId, UserRolesConstant.SuperAdmin)).ReturnsAsync(true);

            _postCommentRepoMock.Setup(r => r.UpdateChildrenStatus(postComment.Id, postComment.Status)).Returns(Task.CompletedTask).Verifiable();
            _postCommentRepoMock.Setup(r => r.Update(postComment)).Verifiable();

            _unitOfWorkMock.Setup(u => u.Complete()).Returns(Task.CompletedTask).Verifiable();

            // ✅ Correctly mocking the generic Map<TSource, TDestination> method
            _mapperMock.Setup(m => m.Map<PostCommentFormViewModel, PostComment>(vm)).Returns(postComment).Verifiable();

            // Act
            var result = await _controller.Edit(1, vm);

            // Assert
            Assert.IsInstanceOf<RedirectToActionResult>(result);
            var redirect = (RedirectToActionResult)result;
            Assert.AreEqual(nameof(PostCommentsController.Index), redirect.ActionName);

            _mapperMock.Verify(m => m.Map<PostCommentFormViewModel, PostComment>(vm), Times.Once);
            _postCommentRepoMock.Verify(r => r.UpdateChildrenStatus(postComment.Id, postComment.Status), Times.Once);
            _postCommentRepoMock.Verify(r => r.Update(postComment), Times.Once);
            _unitOfWorkMock.Verify(u => u.Complete(), Times.Once);
        }

        [Test]
        public async Task Delete_Get_ValidId_ReturnsView()
        {
            // Arrange
            var postComment = new PostComment { Id = 1, UserId = 10 };
            var user = new User { Id = 123 };

            _postCommentRepoMock.Setup(r => r.GetById(1)).ReturnsAsync(postComment);
            _userRepoMock.Setup(r => r.GetByClaimsPrincipal(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(user);
            _userRepoMock.Setup(r => r.IsUserAllowedForOperation(user, postComment.UserId, UserRolesConstant.SuperAdmin)).ReturnsAsync(true);
            _mapperMock.Setup(m => m.Map<PostComment, PostCommentFormViewModel>(postComment))
                .Returns(new PostCommentFormViewModel { Id = 1 });

            // Act
            var result = await _controller.Delete(1);

            // Assert
            Assert.IsInstanceOf<ViewResult>(result);
            var viewResult = (ViewResult)result;
            Assert.IsInstanceOf<PostCommentFormViewModel>(viewResult.Model);
        }

        [Test]
        public async Task Delete_Get_InvalidId_ReturnsNotFound()
        {
            // Arrange
            _postCommentRepoMock.Setup(r => r.GetById(5)).ReturnsAsync((PostComment?)null);

            // Act
            var result = await _controller.Delete(5);

            // Assert
            Assert.IsInstanceOf<NotFoundResult>(result);
        }

        [Test]
        public async Task Delete_Get_UserNotAuthorized_ReturnsUnauthorized()
        {
            // Arrange
            var postComment = new PostComment { Id = 1, UserId = 10 };
            var user = new User { Id = 123 };

            _postCommentRepoMock.Setup(r => r.GetById(1)).ReturnsAsync(postComment);
            _userRepoMock.Setup(r => r.GetByClaimsPrincipal(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(user);
            _userRepoMock.Setup(r => r.IsUserAllowedForOperation(user, postComment.UserId, UserRolesConstant.SuperAdmin)).ReturnsAsync(false);

            // Act
            var result = await _controller.Delete(1);

            // Assert
            Assert.IsInstanceOf<UnauthorizedResult>(result);
        }

        [Test]
        public async Task Delete_PostCommentNotFound_ReturnsNotFound()
        {
            // Arrange
            _unitOfWorkMock
                .Setup(r => r.PostCommentRepository.GetById(It.IsAny<int>()))
                .ReturnsAsync((PostComment)null);

            // Act
            var result = await _controller.Delete(1);

            // Assert
            Assert.IsInstanceOf<NotFoundResult>(result);
        }

        [Test]
        public async Task Delete_Post_EntityNotFound_ReturnsNotFound()
        {
            // Arrange
            var vm = new PostCommentFormViewModel { Id = 1 };
            _postCommentRepoMock.Setup(r => r.GetById(1)).ReturnsAsync((PostComment?)null);

            // Act
            var result = await _controller.Delete(vm.Id);

            // Assert
            Assert.IsInstanceOf<NotFoundResult>(result);
        }

        [Test]
        public async Task Delete_Post_UserNotAuthorized_ReturnsUnauthorized()
        {
            // Arrange
            var postComment = new PostComment { Id = 1, UserId = 10 };
            var vm = new PostCommentFormViewModel { Id = 1 };
            var user = new User { Id = 123 };

            _postCommentRepoMock.Setup(r => r.GetById(1)).ReturnsAsync(postComment);
            _userRepoMock.Setup(r => r.GetByClaimsPrincipal(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(user);
            _userRepoMock.Setup(r => r.IsUserAllowedForOperation(user, postComment.UserId, UserRolesConstant.SuperAdmin)).ReturnsAsync(false);

            // Act
            var result = await _controller.Delete(vm.Id);

            // Assert
            Assert.IsInstanceOf<UnauthorizedResult>(result);
        }

        [Test]
        public async Task DeleteConfirmed_Post_Valid_DeletesAndRedirects()
        {
            // Arrange
            var postComment = new PostComment { Id = 1, UserId = 10 };
            var user = new User { Id = 123 };

            _postCommentRepoMock.Setup(r => r.GetById(1)).ReturnsAsync(postComment);
            _userRepoMock.Setup(r => r.GetByClaimsPrincipal(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(user);
            // Match userId passed in DeleteConfirmed (123)
            _userRepoMock.Setup(r => r.IsUserAllowedForOperation(user, 123, UserRolesConstant.SuperAdmin)).ReturnsAsync(true);

            _postCommentRepoMock.Setup(r => r.Delete(postComment.Id));
            _unitOfWorkMock.Setup(u => u.Complete()).Returns(Task.CompletedTask);

            // Act
            var result = await _controller.DeleteConfirmed(1, 123);
            
            // Assert
            Assert.IsInstanceOf<RedirectToActionResult>(result);
            var redirect = (RedirectToActionResult)result;
            Assert.AreEqual(nameof(PostCommentsController.Index), redirect.ActionName);

            _postCommentRepoMock.Verify(r => r.Delete(postComment.Id), Times.Once);
            _unitOfWorkMock.Verify(u => u.Complete(), Times.Once);
        }
    }
}
