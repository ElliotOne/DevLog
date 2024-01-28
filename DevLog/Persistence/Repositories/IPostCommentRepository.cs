using DevLog.Core.Domain;

namespace DevLog.Persistence.Repositories
{
    /// <summary>
    /// Represents post comments repository interface
    /// </summary>
    public interface IPostCommentRepository
    {
        /// <summary>
        /// Get all post comments
        /// </summary>
        /// <returns>All post comments as IQueryable</returns>
        IQueryable<PostComment> GetAll();

        /// <summary>
        /// Get a post all comments by its identifier
        /// </summary>
        /// <param name="postId">Post identifier</param>
        /// <returns>All post comments for the given post as IEnumerable</returns>
        Task<IEnumerable<PostComment>?> GetAllByPostId(int postId);

        /// <summary>
        /// Get a post comment by identifier
        /// </summary>
        /// <param name="id">Post comment identifier</param>
        /// <returns>A specific post comment or null</returns>
        Task<PostComment?> GetById(int id);

        /// <summary>
        /// Insert a new post comment
        /// </summary>
        /// <param name="postComment">Post comment</param>
        void Insert(PostComment postComment);

        /// <summary>
        /// Update an existing post comment
        /// </summary>
        /// <param name="postComment">Post comment</param>
        void Update(PostComment postComment);

        /// <summary>
        /// Update a post comment all children status
        /// </summary>
        /// <param name="id">Post comment identifier</param>
        /// <param name="postCommentStatus">Post comment status</param>
        Task UpdateChildrenStatus(int id, PostCommentStatus postCommentStatus);

        /// <summary>
        /// Delete a post comment by its identifier
        /// </summary>
        /// <param name="id">Post comment identifier</param>
        Task Delete(int id);

        /// <summary>
        /// Delete a post comment all children
        /// </summary>
        /// <param name="id">Post comment identifier</param>
        Task DeleteChildren(int id);

        /// <summary>
        /// Count total post comments
        /// </summary>
        /// <returns>Number of all post comments</returns>
        Task<int> Count();
    }
}
