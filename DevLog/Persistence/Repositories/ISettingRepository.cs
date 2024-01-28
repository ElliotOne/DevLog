using DevLog.Core.Domain;

namespace DevLog.Persistence.Repositories
{
    /// <summary>
    /// Represents setting repository interface
    /// </summary>
    public interface ISettingRepository
    {
        /// <summary>
        /// Get setting
        /// </summary>
        /// <returns>Setting</returns>
        Task<Setting?> Get();

        /// <summary>
        /// Update setting
        /// </summary>
        /// <param name="setting">Setting</param>
        void Update(Setting setting);
    }
}
