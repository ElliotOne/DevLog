using DevLog.Core.Domain;

namespace DevLog.Persistence.Repositories
{
    /// <summary>
    /// Represents progress bar repository interface
    /// </summary>
    public interface IProgressBarRepository
    {
        /// <summary>
        /// Get all progress bars
        /// </summary>
        /// <returns>All progress bars as IQueryable</returns>
        IQueryable<ProgressBar> GetAll();

        /// <summary>
        /// Get a progress bar by its identifier
        /// </summary>
        /// <param name="id">Progress bar identifier</param>
        /// <returns>A specific progress bar or null</returns>
        Task<ProgressBar?> GetById(int id);

        /// <summary>
        /// Insert a new progress bar
        /// </summary>
        /// <param name="progressBar">Progress bar</param>
        Task Insert(ProgressBar progressBar);

        /// <summary>
        /// Update all existing progress bars
        /// </summary>
        /// <param name="progressBars">Progress bars as IEnumerable</param>
        void UpdateAll(IEnumerable<ProgressBar> progressBars);

        /// <summary>
        /// Delete a progress bar by identifier
        /// </summary>
        /// <param name="id">Progress bar identifier</param>
        Task Delete(int id);

        /// <summary>
        /// Count total progress bars
        /// </summary>
        /// <returns>Number of all progress bars</returns>
        Task<int> Count();
    }
}
