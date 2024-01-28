using DevLog.Core.Domain;
using DevLog.Data;
using DevLog.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;

namespace DevLog.Persistence.EfCoreRepositories
{
    public class PostCommentRepository : IPostCommentRepository
    {
        private readonly IApplicationDbContext _context;

        public PostCommentRepository(IApplicationDbContext context)
        {
            _context = context;
        }

        public IQueryable<PostComment> GetAll()
        {
            return _context.PostComments
                .Include(x => x.Children)
                .Include(x => x.User)
                .Include(x => x.Post)
                .AsQueryable();
        }

        public async Task<IEnumerable<PostComment>?> GetAllByPostId(int postId)
        {
            var postComments = await GetAll()
                .Where(x => x.PostId == postId && x.Status == PostCommentStatus.Accepted)
                .ToListAsync();

            foreach (var comment in postComments)
            {
                if (comment.ParentId != null && !postComments.Exists(x => x.Id == comment.ParentId))
                {
                    comment.ParentId = null;
                }
            }

            return postComments;
        }

        public async Task<PostComment?> GetById(int id)
        {
            return await _context.PostComments
                .Include(x => x.User)
                .Include(x => x.Post)
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        public void Insert(PostComment postComment)
        {
            if (postComment.UserId == 0)
            {
                if (postComment.UserFullName == null)
                {
                    throw new NullReferenceException(nameof(PostComment.UserFullName));
                }

                if (postComment.Email == null)
                {
                    throw new NullReferenceException(nameof(PostComment.Email));
                }
            }

            postComment.IsEdited = false;
            postComment.CreateDate = postComment.LastEditDate = DateTime.Now;

            _context.PostComments.Add(postComment);
        }

        public void Update(PostComment postComment)
        {
            if (postComment.UserId == 0)
            {
                if (postComment.UserFullName == null)
                {
                    throw new NullReferenceException(nameof(PostComment.UserFullName));
                }

                if (postComment.Email == null)
                {
                    throw new NullReferenceException(nameof(PostComment.Email));
                }
            }

            postComment.IsEdited = true;
            postComment.LastEditDate = DateTime.Now;

            _context.PostComments.Update(postComment);
        }

        public async Task UpdateChildrenStatus(int id, PostCommentStatus status)
        {
            if (status != PostCommentStatus.Accepted)
            {
                var children = GetAll().Where(x => x.ParentId == id).ToList();

                foreach (var child in children)
                {
                    await UpdateChildrenStatus(child.Id, status);
                    var currentChild = (await GetById(child.Id))!;

                    currentChild.Status = status;
                    currentChild.IsEdited = true;
                    currentChild.LastEditDate = DateTime.Now;

                    Update(currentChild);
                }
            }
        }

        public async Task Delete(int id)
        {
            var postComment = await GetById(id);

            if (postComment != null)
            {
                _context.PostComments.Remove(postComment);
            }
        }

        public async Task DeleteChildren(int id)
        {
            var children = GetAll()
                .Where(x => x.ParentId == id).ToList();

            foreach (var child in children)
            {
                await DeleteChildren(child.Id);
                await Delete(child.Id);
            }
        }

        public async Task<int> Count()
        {
            return await _context.PostComments.CountAsync();
        }
    }
}
