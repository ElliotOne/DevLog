using DevLog.Core.Domain;

namespace DevLog.Persistence.Repositories
{
    /// <summary>
    /// Represents post repository interface
    /// </summary>
    public interface IPostRepository
    {
        /// <summary>
        /// Get all posts
        /// </summary>
        /// <returns>All posts as IQueryable</returns>
        IQueryable<Post> GetAll();

        /// <summary>
        /// Get a post by its identifier
        /// </summary>
        /// <param name="id">Post identifier</param>
        /// <returns>A specific post or null</returns>
        Task<Post?> GetById(int id);

        /// <summary>
        /// Insert a new post
        /// </summary>
        /// <param name="post">Post</param>
        void Insert(Post post);

        /// <summary>
        /// Update an existing post
        /// </summary>
        /// <param name="post">Post</param>
        void Update(Post post);

        /// <summary>
        /// Delete a post by identifier
        /// </summary>
        /// <param name="id">Post identifier</param>
        Task Delete(int id);

        /// <summary>
        /// Count total posts
        /// </summary>
        /// <returns>Number of all posts</returns>
        Task<int> Count();
    }
}
