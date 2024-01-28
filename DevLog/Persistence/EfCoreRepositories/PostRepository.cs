using DevLog.Core.Domain;
using DevLog.Data;
using DevLog.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;

namespace DevLog.Persistence.EfCoreRepositories
{
    public class PostRepository : IPostRepository
    {
        private readonly IApplicationDbContext _context;

        public PostRepository(IApplicationDbContext context)
        {
            _context = context;
        }

        public IQueryable<Post> GetAll()
        {
            return _context.Posts
                .Include(x => x.PostCategory)
                .Include(x => x.User)
                .AsQueryable();
        }

        public async Task<Post?> GetById(int id)
        {
            return await _context.Posts
                .Include(x => x.PostCategory)
                .Include(x => x.User)
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        public void Insert(Post post)
        {
            post.CreateDate = post.LastEditDate = DateTime.Now;
            _context.Posts.Add(post);
        }

        public void Update(Post post)
        {
            post.LastEditDate = DateTime.Now;
            _context.Posts.Update(post);
        }

        public async Task Delete(int id)
        {
            var post = await GetById(id);

            if (post != null)
            {
                _context.Posts.Remove(post);
            }
        }

        public async Task<int> Count()
        {
            return await _context.Posts.CountAsync();
        }
    }
}
