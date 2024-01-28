using AutoMapper;
using DevLog.Core.Domain;
using DevLog.Infrastructure;
using DevLog.Models.Constants;
using DevLog.Models.Parameters;
using DevLog.Models.Shared.JsonResults;
using DevLog.Models.Shared.Pagination;
using DevLog.Models.ViewModels;
using DevLog.Persistence;
using Microsoft.AspNetCore.Mvc;
using SmartBreadcrumbs.Attributes;

namespace DevLog.Controllers
{
    public class BlogController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _accessor;

        public BlogController(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IHttpContextAccessor accessor)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _accessor = accessor;
        }

        [Breadcrumb("Blog")]
        public IActionResult Index(
            int? page,
            string? searchString,
            int? postCategoryId,
            string? postTag,
            PostSortFilterType postSortFilterType = PostSortFilterType.SortByDateDesc)
        {
            var postSearchParameters = new PostSearchParameters()
            {
                SearchString = searchString,
                PostCategoryId = postCategoryId,
                PostTag = postTag,
                PostSortFilterType = postSortFilterType,
            };

            const int pageSize = 6;

            var posts =
                GetPosts(postSearchParameters, pageIndex: page - 1 ?? 0, pageSize: pageSize);

            var blogViewModel = new PostsViewModel()
            {
                PostViewModels = _mapper.Map<IEnumerable<Post>, IEnumerable<PostViewModel>>(posts),
                Pager = new Pager(posts.TotalCount, posts.TotalPages, page, pageSize),
                SearchString = searchString,
                PostCategoryId = postCategoryId,
                PostTag = postTag,
                PostSortFilterType = postSortFilterType
            };

            return View(blogViewModel);
        }

        [Breadcrumb("ViewData.Title")]
        public async Task<IActionResult> Post(int id)
        {
            var post = await _unitOfWork.PostRepository.GetById(id);

            if (post == null)
            {
                return NotFound();
            }

            var postViewModel = _mapper.Map<Post, PostViewModel>(post);

            var postComments = await _unitOfWork.PostCommentRepository
                .GetAllByPostId(post.Id);

            postViewModel.PostCommentViewModels =
                _mapper.Map<IEnumerable<PostComment>, IEnumerable<PostCommentViewModel>>(postComments!);

            return View(postViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CommentPost(PostCommentFormViewModel postCommentFormViewModel)
        {
            if (!ModelState.IsValid)
            {
                return new JsonResult(new JsonResultModel()
                {
                    StatusCode = JsonResultStatusCode.ModelStateIsNotValid,
                    Message = MessagesConstant.CommentFailedToSend
                });
            }

            var postComment = _mapper.Map<PostCommentFormViewModel, PostComment>(postCommentFormViewModel);

            if (_accessor.HttpContext?.Connection.RemoteIpAddress != null)
            {
                postComment.Ip = _accessor.HttpContext.Connection.RemoteIpAddress.ToString();
            }

            postComment.Status = PostCommentStatus.Unclear;

            _unitOfWork.PostCommentRepository.Insert(postComment);
            await _unitOfWork.Complete();

            return new JsonResult(new JsonResultModel()
            {
                StatusCode = JsonResultStatusCode.Success,
                Message = MessagesConstant.CommentSentAndWillShowAfterAcceptance
            });
        }

        [NonAction]
        public PagedList<Post> GetPosts(
            PostSearchParameters? postSearchDto = null, int pageIndex = 0, int pageSize = int.MaxValue)
        {
            var query = _unitOfWork.PostRepository.GetAll();

            if (postSearchDto != null)
            {
                if (postSearchDto.PostCategoryId != null)
                {
                    query = query.Where(x => x.PostCategoryId == postSearchDto.PostCategoryId);
                }

                if (!string.IsNullOrWhiteSpace(postSearchDto.PostTag))
                {
                    query = query.Where(x => x.Tags != null && x.Tags.Contains(postSearchDto.PostTag));
                }

                if (!string.IsNullOrWhiteSpace(postSearchDto.SearchString))
                {
                    string searchString = postSearchDto.SearchString.ToLower();

                    query = query.Where(x =>
                        (x.Title.Contains(searchString)) ||
                        (x.Tags != null && x.Tags.Contains(searchString)) ||
                        (x.PostCategory != null && x.PostCategory.Title.Contains(searchString)) ||
                        (x.Body.Contains(searchString))
                        );
                }

                query = postSearchDto.PostSortFilterType switch
                {
                    PostSortFilterType.SortByDateAsc => query.OrderBy(x => x.CreateDate),
                    PostSortFilterType.SortByDateDesc => query.OrderByDescending(x => x.CreateDate),
                    _ => query.OrderByDescending(x => x.CreateDate)
                };
            }
            else
            {
                query = query.OrderByDescending(x => x.CreateDate);
            }

            return new PagedList<Post>(query, pageIndex, pageSize);
        }
    }
}