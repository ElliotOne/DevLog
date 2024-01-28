using AutoMapper;
using DevLog.Core.Domain;
using DevLog.Models.ViewModels;
using DevLog.Persistence;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DevLog.ViewComponents
{
    public class LatestPostsViewComponent : ViewComponent
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public LatestPostsViewComponent(IUnitOfWork unitOfWork,IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var posts = await _unitOfWork.PostRepository
                .GetAll()
                .OrderByDescending(x => x.CreateDate)
                .Take(2)
                .ToListAsync();

            return View(_mapper.Map<IEnumerable<Post>, IEnumerable<PostViewModel>>(posts));
        }
    }
}
