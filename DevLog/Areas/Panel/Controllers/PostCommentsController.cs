using AutoMapper;
using DevLog.Areas.Panel.Extensions;
using DevLog.Areas.Panel.Models;
using DevLog.Areas.Panel.Models.ViewModels;
using DevLog.Core.Domain;
using DevLog.Models.Constants;
using DevLog.Persistence;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PostCommentFormViewModel = DevLog.Areas.Panel.Models.ViewModels.PostCommentFormViewModel;

namespace DevLog.Areas.Panel.Controllers
{
    [Area(AreasConstant.Panel)]
    [Authorize(Roles = UserRolesConstant.SuperAdmin + "," + UserRolesConstant.Admin + "," + UserRolesConstant.Writer)]
    public class PostCommentsController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _accessor;
        public PostCommentsController(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IHttpContextAccessor accessor
        )
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _accessor = accessor;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> LoadPostCommentsTable([FromBody] DTParameters dtParameters)
        {
            var searchBy = dtParameters.Search?.Value;

            string orderCriteria;
            bool orderAscendingDirection;

            if (dtParameters.Order == null)
            {
                //When Empty search will be order the results by Id ascending
                orderCriteria = nameof(IEntity.Id);
                orderAscendingDirection = true;
            }
            else
            {
                //Default sort on the 1st column
                orderCriteria = dtParameters.Columns![dtParameters.Order[0].Column].Data!;
                orderAscendingDirection =
                    string.Equals(
                        dtParameters.Order[0].Dir.ToString(),
                        nameof(LinqExtensions.Order.Asc),
                        StringComparison.CurrentCultureIgnoreCase);
            }

            var result = _unitOfWork.PostCommentRepository.GetAll();

            var user = (await _unitOfWork.UserRepository.GetByClaimsPrincipal(HttpContext.User))!;

            if (!await _unitOfWork.UserRepository.IsInRole(user, UserRolesConstant.SuperAdmin))
            {
                result = result.Where(x => x.Post.UserId == user.Id);
            }

            if (!string.IsNullOrWhiteSpace(searchBy))
            {
                result = result.Where(x =>
                    (x.UserFullName != null && x.UserFullName.Contains(searchBy)) ||
                    (x.Post.Title.Contains(searchBy)) ||
                    (x.Body.Contains(searchBy)) ||
                    (x.Email != null && x.Email.Contains(searchBy)) ||
                    (x.Ip.Contains(searchBy)) ||
                    (x.CreateDate.ToString("F").Contains(searchBy)) ||
                    (x.LastEditDate.ToString("F").Contains(searchBy))
                );
            }

            if (string.Equals(orderCriteria,
                nameof(PostCommentIndexViewModel.UserFullName), StringComparison.InvariantCultureIgnoreCase))
            {
                result = orderAscendingDirection
                    ? result.OrderBy(x => x.User == null ? x.UserFullName : x.User.FirstName + " " + x.User.LastName)
                    : result.OrderByDescending(x => x.User == null ? x.UserFullName : x.User.FirstName + " " + x.User.LastName);
            }
            else if (string.Equals(orderCriteria,
                nameof(PostCommentIndexViewModel.PostTitle), StringComparison.InvariantCultureIgnoreCase))
            {
                result = orderAscendingDirection
                    ? result.OrderBy(x => x.Post.Title)
                    : result.OrderByDescending(x => x.Post.Title);
            }
            else
            {
                result = orderAscendingDirection ?
                    result.OrderByDynamic(orderCriteria, LinqExtensions.Order.Asc) :
                    result.AsQueryable().OrderByDynamic(orderCriteria, LinqExtensions.Order.Desc);
            }

            var filteredResultsCount = result.Count();
            var totalResultsCount = await _unitOfWork.PostCommentRepository.Count();

            var resultList = result
                .Skip(dtParameters.Start)
                .Take(dtParameters.Length)
                .ToList();

            return new JsonResult(new DatatableJsonResultModel()
            {
                Draw = dtParameters.Draw,
                RecordsTotal = totalResultsCount,
                RecordsFiltered = filteredResultsCount,
                Data =
                    _mapper.Map<IEnumerable<PostComment>, IEnumerable<PostCommentIndexViewModel>>(resultList)
            });
        }

        public async Task<IActionResult> Details(int id)
        {
            var postComment = await _unitOfWork.PostCommentRepository.GetById(id);

            if (postComment == null)
            {
                return NotFound();
            }

            var user = (await _unitOfWork.UserRepository.GetByClaimsPrincipal(HttpContext.User))!;

            if (!await _unitOfWork.UserRepository
                .IsUserAllowedForOperation(user, postComment.UserId, UserRolesConstant.SuperAdmin))
            {
                return Unauthorized();
            }

            return View(_mapper.Map<PostComment, PostCommentFormViewModel>(postComment));
        }

        public async Task<IActionResult> Reply(int id)
        {
            var parentPostComment = await _unitOfWork.PostCommentRepository.GetById(id);

            if (parentPostComment == null)
            {
                return NotFound();
            }

            var user = (await _unitOfWork.UserRepository.GetByClaimsPrincipal(HttpContext.User))!;

            if (!await _unitOfWork.UserRepository
                .IsUserAllowedForOperation(user, parentPostComment.UserId, UserRolesConstant.SuperAdmin))
            {
                return Unauthorized();
            }

            return View(_mapper.Map<PostComment, PostCommentReplyFormViewModel>(parentPostComment));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Reply(int id, PostCommentReplyFormViewModel postCommentReplyFormViewModel)
        {
            if (id != postCommentReplyFormViewModel.Id)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                return View(postCommentReplyFormViewModel);
            }

            var parentPostComment = await _unitOfWork.PostCommentRepository
                .GetById(postCommentReplyFormViewModel.ParentId);

            if (parentPostComment == null)
            {
                return NotFound();
            }

            var user = (await _unitOfWork.UserRepository.GetByClaimsPrincipal(HttpContext.User))!;

            if (!await _unitOfWork.UserRepository
                    .IsUserAllowedForOperation(user, parentPostComment.UserId, UserRolesConstant.SuperAdmin))
            {
                return Unauthorized();
            }

            var postComment = _mapper.Map<PostCommentReplyFormViewModel, PostComment>(postCommentReplyFormViewModel);

            postComment.UserId = user.Id;
            postComment.Ip = _accessor.HttpContext?.Connection.RemoteIpAddress?.ToString() ?? string.Empty;

            _unitOfWork.PostCommentRepository.Insert(postComment);
            await _unitOfWork.Complete();

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int id)
        {
            var postComment = await _unitOfWork.PostCommentRepository.GetById(id);

            if (postComment == null)
            {
                return NotFound();
            }

            var user = (await _unitOfWork.UserRepository.GetByClaimsPrincipal(HttpContext.User))!;

            if (!await _unitOfWork.UserRepository
                .IsUserAllowedForOperation(user, postComment.UserId, UserRolesConstant.SuperAdmin))
            {
                return Unauthorized();
            }

            return View(_mapper.Map<PostComment, PostCommentFormViewModel>(postComment));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, PostCommentFormViewModel postCommentFormViewModel)
        {
            if (id != postCommentFormViewModel.Id)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                return View(postCommentFormViewModel);
            }

            var user = (await _unitOfWork.UserRepository.GetByClaimsPrincipal(HttpContext.User))!;

            if (!await _unitOfWork.UserRepository
                    .IsUserAllowedForOperation(user, postCommentFormViewModel.PostUserId, UserRolesConstant.SuperAdmin))
            {
                return Unauthorized();
            }

            var postComment = _mapper.Map<PostCommentFormViewModel, PostComment>(postCommentFormViewModel);

            await _unitOfWork.PostCommentRepository.UpdateChildrenStatus(postComment.Id, postComment.Status);
            _unitOfWork.PostCommentRepository.Update(postComment);
            await _unitOfWork.Complete();

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int id)
        {
            var postComment = await _unitOfWork.PostCommentRepository.GetById(id);

            if (postComment == null)
            {
                return NotFound();
            }

            var user = (await _unitOfWork.UserRepository.GetByClaimsPrincipal(HttpContext.User))!;

            if (!await _unitOfWork.UserRepository
                .IsUserAllowedForOperation(user, postComment.UserId, UserRolesConstant.SuperAdmin))
            {
                return Unauthorized();
            }

            return View(_mapper.Map<PostComment, PostCommentFormViewModel>(postComment));
        }

        [HttpPost, ActionName(nameof(Delete))]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id, int userId)
        {
            var user = (await _unitOfWork.UserRepository.GetByClaimsPrincipal(HttpContext.User))!;

            if (!await _unitOfWork.UserRepository
                .IsUserAllowedForOperation(user, userId, UserRolesConstant.SuperAdmin))
            {
                return Unauthorized();
            }

            await _unitOfWork.PostCommentRepository.DeleteChildren(id);
            await _unitOfWork.PostCommentRepository.Delete(id);
            await _unitOfWork.Complete();

            return RedirectToAction(nameof(Index));
        }
    }
}
