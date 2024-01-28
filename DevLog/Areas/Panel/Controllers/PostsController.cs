using AutoMapper;
using DevLog.Areas.Panel.Extensions;
using DevLog.Areas.Panel.Models;
using DevLog.Areas.Panel.Models.ViewModels;
using DevLog.Core.Domain;
using DevLog.Models.Constants;
using DevLog.Persistence;
using DevLog.Services.FileHandler;
using DevLog.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace DevLog.Areas.Panel.Controllers
{
    [Area(AreasConstant.Panel)]
    [Authorize(Roles = UserRolesConstant.SuperAdmin + "," + UserRolesConstant.Admin + "," + UserRolesConstant.Writer)]
    public class PostsController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IFileHandler _fileHandler;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public PostsController(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IFileHandler fileHandler,
            IWebHostEnvironment webHostEnvironment)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _fileHandler = fileHandler;
            _webHostEnvironment = webHostEnvironment;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> LoadPostsTable([FromBody] DTParameters dtParameters)
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

            var result = _unitOfWork.PostRepository.GetAll();

            var user = (await _unitOfWork.UserRepository.GetByClaimsPrincipal(HttpContext.User))!;

            if (!await _unitOfWork.UserRepository.IsInRole(user, UserRolesConstant.SuperAdmin))
            {
                result = result.Where(x => x.UserId == user.Id);
            }

            if (!string.IsNullOrWhiteSpace(searchBy))
            {
                result = result.Where(x =>
                    (x.Title.Contains(searchBy)) ||
                    ((x.User.FirstName + " " + x.User.LastName).Contains(searchBy)) ||
                    (x.PostCategory != null && x.PostCategory.Title.Contains(searchBy)) ||
                    (x.Body.Contains(searchBy)) ||
                    (x.Tags != null && x.Tags.Contains(searchBy)) ||
                    (x.CreateDate.ToString("F").Contains(searchBy)) ||
                    (x.LastEditDate.ToString("F").Contains(searchBy))
                );
            }

            if (string.Equals(orderCriteria,
                nameof(PostIndexViewModel.PostCategoryTitle), StringComparison.InvariantCultureIgnoreCase))
            {
                result = orderAscendingDirection
                    ? result.OrderBy(x => x.PostCategory != null ? x.PostCategory.Title : null)
                    : result.OrderByDescending(x => x.PostCategory != null ? x.PostCategory.Title : null);
            }
            else if (string.Equals(orderCriteria,
                nameof(PostIndexViewModel.UserFullName), StringComparison.InvariantCultureIgnoreCase))
            {
                result = orderAscendingDirection
                    ? result.OrderBy(x => x.User.FirstName + " " + x.User.LastName)
                    : result.OrderByDescending(x => x.User.FirstName + " " + x.User.LastName);
            }
            else
            {
                result = orderAscendingDirection ?
                    result.OrderByDynamic(orderCriteria, LinqExtensions.Order.Asc) :
                    result.AsQueryable().OrderByDynamic(orderCriteria, LinqExtensions.Order.Desc);
            }

            var filteredResultsCount = result.Count();
            var totalResultsCount = await _unitOfWork.PostRepository.Count();

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
                    _mapper.Map<IEnumerable<Post>, IEnumerable<PostIndexViewModel>>(resultList)
            });
        }

        public async Task<IActionResult> Details(int id)
        {
            var post = await _unitOfWork.PostRepository.GetById(id);

            if (post == null)
            {
                return NotFound();
            }

            var user = (await _unitOfWork.UserRepository.GetByClaimsPrincipal(HttpContext.User))!;

            if (!await _unitOfWork.UserRepository
                .IsUserAllowedForOperation(user, post.UserId, UserRolesConstant.SuperAdmin))
            {
                return Unauthorized();
            }

            ViewData[nameof(PostFormViewModel.PostCategoryId)] = GetPostCategories();
            return View(_mapper.Map<Post, PostFormViewModel>(post));
        }

        public IActionResult Create()
        {
            ViewData[nameof(PostFormViewModel.PostCategoryId)] = GetPostCategories();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(PostFormViewModel postFormViewModel)
        {
            if (!ModelState.IsValid)
            {
                ViewData[nameof(PostFormViewModel.PostCategoryId)] = GetPostCategories();
                return View(postFormViewModel);
            }

            var post = _mapper.Map<PostFormViewModel, Post>(postFormViewModel);

            var user = (await _unitOfWork.UserRepository.GetByClaimsPrincipal(HttpContext.User))!;

            post.UserId = user.Id;

            var files = HttpContext.Request.Form.Files;

            if (files.Any())
            {
                post.ImageVirtualPath =
                    _fileHandler.GetRelativePath(
                        files[0].FileName,
                        GuidUtility.NewGuidSafeString(),
                        FileType.Post);

                await _fileHandler.Upload(
                    files[0],
                    post.ImageVirtualPath);
            }

            _unitOfWork.PostRepository.Insert(post);
            await _unitOfWork.Complete();

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int id)
        {
            var post = await _unitOfWork.PostRepository.GetById(id);

            if (post == null)
            {
                return NotFound();
            }

            var user = (await _unitOfWork.UserRepository.GetByClaimsPrincipal(HttpContext.User))!;

            if (!await _unitOfWork.UserRepository
                .IsUserAllowedForOperation(user, post.UserId, UserRolesConstant.SuperAdmin))
            {
                return Unauthorized();
            }

            ViewData[nameof(PostFormViewModel.PostCategoryId)] = GetPostCategories();
            return View(_mapper.Map<Post, PostFormViewModel>(post));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, PostFormViewModel postFormViewModel)
        {
            if (id != postFormViewModel.Id)
            {
                return BadRequest();
            }

            if (ModelState.IsValid)
            {
                ViewData[nameof(PostFormViewModel.PostCategoryId)] = GetPostCategories();
                return View(postFormViewModel);
            }

            var user = (await _unitOfWork.UserRepository.GetByClaimsPrincipal(HttpContext.User))!;

            if (!await _unitOfWork.UserRepository
                    .IsUserAllowedForOperation(user, postFormViewModel.UserId, UserRolesConstant.SuperAdmin))
            {
                return Unauthorized();
            }

            var post = _mapper.Map<PostFormViewModel, Post>(postFormViewModel);

            var files = HttpContext.Request.Form.Files;

            if (files.Any())
            {
                //Delete the old file
                if (!string.IsNullOrWhiteSpace(post.ImageVirtualPath))
                {
                    _fileHandler.Delete(post.ImageVirtualPath);
                }

                //Upload the new file
                post.ImageVirtualPath =
                    _fileHandler.GetRelativePath(
                        files[0].FileName,
                        GuidUtility.NewGuidSafeString(),
                        FileType.Post);

                await _fileHandler.Upload(
                    files[0],
                    post.ImageVirtualPath);
            }

            _unitOfWork.PostRepository.Update(post);
            await _unitOfWork.Complete();

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int id)
        {
            var post = await _unitOfWork.PostRepository.GetById(id);

            if (post == null)
            {
                return NotFound();
            }

            var user = (await _unitOfWork.UserRepository.GetByClaimsPrincipal(HttpContext.User))!;

            if (!await _unitOfWork.UserRepository
                .IsUserAllowedForOperation(user, post.UserId, UserRolesConstant.SuperAdmin))
            {
                return Unauthorized();
            }

            ViewData[nameof(PostFormViewModel.PostCategoryId)] = GetPostCategories();
            return View(_mapper.Map<Post, PostFormViewModel>(post));
        }

        [HttpPost, ActionName(nameof(Delete))]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id, int userId, string? imageVirtualPath)
        {
            var user = (await _unitOfWork.UserRepository.GetByClaimsPrincipal(HttpContext.User))!;

            if (!await _unitOfWork.UserRepository
                .IsUserAllowedForOperation(user, userId, UserRolesConstant.SuperAdmin))
            {
                return Unauthorized();
            }

            if (imageVirtualPath == null)
            {
                return BadRequest();
            }

            _fileHandler.Delete(imageVirtualPath);

            await _unitOfWork.PostRepository.Delete(id);
            await _unitOfWork.Complete();

            return RedirectToAction(nameof(Index));
        }

        [NonAction]
        public SelectList GetPostCategories()
        {
            return new SelectList(
                _unitOfWork.PostCategoryRepository.GetAll(),
                "Id",
                "Title"
            );
        }
    }
}
