using AutoMapper;
using DevLog.Core.Domain;
using DevLog.Models.ViewModels;
using DevLog.Persistence;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DevLog.ViewComponents
{
    public class CertificatesViewComponent : ViewComponent
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public CertificatesViewComponent(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var certificates =
                await _unitOfWork.CertificateRepository.GetAll().ToListAsync();

            return View(_mapper.Map<IEnumerable<Certificate>, IEnumerable<CertificateViewModel>>(certificates));
        }
    }
}
