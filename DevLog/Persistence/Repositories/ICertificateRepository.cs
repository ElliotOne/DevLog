using DevLog.Core.Domain;

namespace DevLog.Persistence.Repositories
{
    /// <summary>
    /// Represents certificate repository interface
    /// </summary>
    public interface ICertificateRepository
    {
        /// <summary>
        /// Get all certificates
        /// </summary>
        /// <returns>All certificates as IQueryable</returns>
        IQueryable<Certificate> GetAll();

        /// <summary>
        /// Get a certificate by its identifier
        /// </summary>
        /// <param name="id">Certificate identifier</param>
        /// <returns>A specific certificate or null</returns>
        Task<Certificate?> GetById(int id);

        /// <summary>
        /// Insert a new certificate
        /// </summary>
        /// <param name="certificate">Certificate</param>
        void Insert(Certificate certificate);

        /// <summary>
        /// Update an existing certificate
        /// </summary>
        /// <param name="certificate">Certificate</param>
        void Update(Certificate certificate);

        /// <summary>
        /// Delete a certificate by its identifier
        /// </summary>
        /// <param name="id">Certificate identifier</param>
        Task Delete(int id);
    }
}
