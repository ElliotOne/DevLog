using DevLog.Core.Domain;

namespace DevLog.Persistence.Repositories
{
    /// <summary>
    /// Represents contact repository interface
    /// </summary>
    public interface IContactRepository
    {
        /// <summary>
        /// Get all contacts
        /// </summary>
        /// <returns>All contacts as IQueryable</returns>
        IQueryable<Contact> GetAll();

        /// <summary>
        /// Get a contact by its identifier
        /// </summary>
        /// <param name="id">Contact identifier</param>
        /// <returns>A specific contact or null</returns>
        Task<Contact?> GetById(int id);

        /// <summary>
        /// Insert a new contact
        /// </summary>
        /// <param name="contact">Contact</param>
        void Insert(Contact contact);

        /// <summary>
        /// Update an existing contact
        /// </summary>
        /// <param name="contact">Contact</param>
        void Update(Contact contact);

        /// <summary>
        /// Delete a contact by its identifier
        /// </summary>
        /// <param name="id">Certificate identifier</param>
        Task Delete(int id);

        /// <summary>
        /// Count total contacts
        /// </summary>
        /// <returns>Number of all contacts</returns>
        Task<int> Count();
    }
}
