using DevLog.Core.Domain;
using DevLog.Data;
using DevLog.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;

namespace DevLog.Persistence.EfCoreRepositories
{
    public class SettingRepository : ISettingRepository
    {
        private readonly IApplicationDbContext _context;

        public SettingRepository(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Setting?> Get()
        {
            return await _context.Settings.FirstOrDefaultAsync();
        }

        public void Update(Setting setting)
        {
            _context.Settings.Update(setting);
        }
    }
}
