using DevLog.Core.Domain;
using DevLog.Data;
using DevLog.Persistence.Repositories;

namespace DevLog.Persistence.EfCoreRepositories
{
    public class CertificateRepository : ICertificateRepository
    {
        private readonly IApplicationDbContext _context;

        public CertificateRepository(IApplicationDbContext context)
        {
            _context = context;
        }

        public IQueryable<Certificate> GetAll()
        {
            return _context.Certificates.AsQueryable();
        }

        public async Task<Certificate?> GetById(int id)
        {
            return await _context.Certificates.FindAsync(id);
        }

        public void Insert(Certificate certificate)
        {
            certificate.CreateDate = certificate.LastEditDate = DateTime.Now;
            _context.Certificates.Add(certificate);
        }

        public void Update(Certificate certificate)
        {
            certificate.LastEditDate = DateTime.Now;
            _context.Certificates.Update(certificate);
        }

        public async Task Delete(int id)
        {
            var certificate = await GetById(id);

            if (certificate != null)
            {
                _context.Certificates.Remove(certificate);
            }
        }
    }
}
