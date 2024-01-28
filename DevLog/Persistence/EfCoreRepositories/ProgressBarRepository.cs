using DevLog.Core.Domain;
using DevLog.Data;
using DevLog.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;

namespace DevLog.Persistence.EfCoreRepositories
{
    public class ProgressBarRepository : IProgressBarRepository
    {
        private readonly IApplicationDbContext _context;

        public ProgressBarRepository(IApplicationDbContext context)
        {
            _context = context;
        }

        public IQueryable<ProgressBar> GetAll()
        {
            return _context.ProgressBars.AsQueryable();
        }

        public async Task<ProgressBar?> GetById(int id)
        {
            return await _context.ProgressBars.FindAsync(id);
        }

        public async Task Insert(ProgressBar progressBar)
        {
            var maxIndex = await _context.ProgressBars
                .MaxAsync(x => (int?)x.SortIndex) ?? 0;

            progressBar.SortIndex = (maxIndex == 0 && await Count() == 0) ? 0 : maxIndex + 1;

            _context.ProgressBars.Add(progressBar);
        }

        public void UpdateAll(IEnumerable<ProgressBar> progressBars)
        {
            _context.ProgressBars.UpdateRange(progressBars);
        }

        public async Task Delete(int id)
        {
            var progressBar = await GetById(id);

            if (progressBar != null)
            {
                _context.ProgressBars.Remove(progressBar);
            }
        }

        public async Task<int> Count()
        {
            return await _context.ProgressBars.CountAsync();
        }
    }
}
