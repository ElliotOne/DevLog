using DevLog.Core.Domain;

namespace DevLog.Persistence.Repositories
{
    /// <summary>
    /// Represents post category repository interface
    /// </summary>
    public interface IPostCategoryRepository
    {
        /// <summary>
        /// Get all post categories
        /// </summary>
        /// <returns>All post categories as IQueryable</returns>
        IQueryable<PostCategory> GetAll();

        /// <summary>
        /// Get a post category by its identifier
        /// </summary>
        /// <param name="id">Post category identifier</param>
        /// <returns>A specific post category or null</returns>
        Task<PostCategory?> GetById(int id);

        /// <summary>
        /// Insert a new post category
        /// </summary>
        /// <param name="postCategory">Post category</param>
        void Insert(PostCategory postCategory);

        /// <summary>
        /// Update an existing post category
        /// </summary>
        /// <param name="postCategory">Post category</param>
        void Update(PostCategory postCategory);

        /// <summary>
        /// Delete a post category by its identifier
        /// </summary>
        /// <param name="id">Post category identifier</param>
        Task Delete(int id);

        /// <summary>
        /// Check if the post category currently exists or not
        /// </summary>
        /// <param name="postCategory">Post category</param>
        /// <returns>True if it exists; otherwise, false</returns>
        Task<bool> IsPostCategoryExists(PostCategory postCategory);

        /// <summary>
        /// Count total post categories
        /// </summary>
        /// <returns>Number of all post categories</returns>
        Task<int> Count();
    }
}
