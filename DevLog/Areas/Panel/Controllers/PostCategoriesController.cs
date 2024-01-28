using AutoMapper;
using DevLog.Areas.Panel.Extensions;
using DevLog.Areas.Panel.Models;
using DevLog.Areas.Panel.Models.ViewModels;
using DevLog.Core.Domain;
using DevLog.Models.Constants;
using DevLog.Persistence;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DevLog.Areas.Panel.Controllers
{
    [Area(AreasConstant.Panel)]
    [Authorize(Roles = UserRolesConstant.SuperAdmin + "," + UserRolesConstant.Admin)]
    public class PostCategoriesController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public PostCategoriesController(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> LoadPostCategoriesTable([FromBody] DTParameters dtParameters)
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

            var result = _unitOfWork.PostCategoryRepository.GetAll();

            if (!string.IsNullOrWhiteSpace(searchBy))
            {
                result = result.Where(x =>
                    (x.Title.Contains(searchBy)) ||
                    (x.CreateDate.ToString("F").Contains(searchBy)) ||
                    (x.LastEditDate.ToString("F").Contains(searchBy))
                );
            }

            result = orderAscendingDirection ?
                result.OrderByDynamic(orderCriteria, LinqExtensions.Order.Asc) :
                result.AsQueryable().OrderByDynamic(orderCriteria, LinqExtensions.Order.Desc);

            var filteredResultsCount = result.Count();
            var totalResultsCount = await _unitOfWork.PostCategoryRepository.Count();

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
                    _mapper.Map<IEnumerable<PostCategory>, IEnumerable<PostCategoryIndexViewModel>>(resultList)
            });
        }

        public async Task<IActionResult> Details(int id)
        {
            var postCategory = await _unitOfWork.PostCategoryRepository.GetById(id);

            if (postCategory == null)
            {
                return NotFound();
            }

            return View(_mapper.Map<PostCategory, PostCategoryFormViewModel>(postCategory));
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(PostCategoryFormViewModel postCategoryFormViewModel)
        {
            if (!ModelState.IsValid)
            {
                return View(postCategoryFormViewModel);
            }

            var postCategory = _mapper.Map<PostCategoryFormViewModel, PostCategory>(postCategoryFormViewModel);

            if (await _unitOfWork.PostCategoryRepository.IsPostCategoryExists(postCategory))
            {
                ModelState.AddModelError(nameof(PostCategoryFormViewModel.Title),
                    string.Format(MessagesConstant.DuplicateError, nameof(PostCategoryFormViewModel.Title)));
                return View(postCategoryFormViewModel);
            }

            _unitOfWork.PostCategoryRepository.Insert(postCategory);
            await _unitOfWork.Complete();

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int id)
        {
            var postCategory = await _unitOfWork.PostCategoryRepository.GetById(id);

            if (postCategory == null)
            {
                return NotFound();
            }

            return View(_mapper.Map<PostCategory, PostCategoryFormViewModel>(postCategory));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, PostCategoryFormViewModel postCategoryFormViewModel)
        {
            if (id != postCategoryFormViewModel.Id)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                return View(postCategoryFormViewModel);
            }

            var postCategory = _mapper.Map<PostCategoryFormViewModel, PostCategory>(postCategoryFormViewModel);

            if (await _unitOfWork.PostCategoryRepository.IsPostCategoryExists(postCategory))
            {
                ModelState.AddModelError(nameof(PostCategoryFormViewModel.Title),
                    string.Format(MessagesConstant.DuplicateError, nameof(PostCategoryFormViewModel.Title)));
                return View(postCategoryFormViewModel);
            }

            _unitOfWork.PostCategoryRepository.Update(postCategory);
            await _unitOfWork.Complete();

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int id)
        {
            var postCategory = await _unitOfWork.PostCategoryRepository.GetById(id);

            if (postCategory == null)
            {
                return NotFound();
            }

            return View(_mapper.Map<PostCategory, PostCategoryFormViewModel>(postCategory));
        }

        [HttpPost, ActionName(nameof(Delete))]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _unitOfWork.PostCategoryRepository.Delete(id);
            await _unitOfWork.Complete();

            return RedirectToAction(nameof(Index));
        }
    }
}
