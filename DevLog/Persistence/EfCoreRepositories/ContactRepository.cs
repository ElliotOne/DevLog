using DevLog.Core.Domain;
using DevLog.Data;
using DevLog.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;

namespace DevLog.Persistence.EfCoreRepositories
{
    public class ContactRepository : IContactRepository
    {
        private readonly IApplicationDbContext _context;

        public ContactRepository(IApplicationDbContext context)
        {
            _context = context;
        }

        public IQueryable<Contact> GetAll()
        {
            return _context.Contacts.AsQueryable();
        }

        public async Task<Contact?> GetById(int id)
        {
            return await _context.Contacts.FindAsync(id);
        }

        public void Insert(Contact contact)
        {
            contact.CreateDate = contact.LastEditDate = DateTime.Now;
            _context.Contacts.Add(contact);
        }

        public void Update(Contact contact)
        {
            contact.LastEditDate = DateTime.Now;
            _context.Contacts.Update(contact);
        }

        public async Task Delete(int id)
        {
            var contact = await GetById(id);

            if (contact != null)
            {
                _context.Contacts.Remove(contact);
            }
        }

        public async Task<int> Count()
        {
            return await _context.Contacts.CountAsync();
        }
    }
}
