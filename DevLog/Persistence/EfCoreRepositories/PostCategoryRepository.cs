using DevLog.Core.Domain;
using DevLog.Data;
using DevLog.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;

namespace DevLog.Persistence.EfCoreRepositories
{
    public class PostCategoryRepository : IPostCategoryRepository
    {
        private readonly IApplicationDbContext _context;

        public PostCategoryRepository(IApplicationDbContext context)
        {
            _context = context;
        }

        public IQueryable<PostCategory> GetAll()
        {
            return _context.PostCategories.AsQueryable();
        }

        public async Task<PostCategory?> GetById(int id)
        {
            return await _context.PostCategories.FindAsync(id);
        }

        public void Insert(PostCategory postCategory)
        {
            postCategory.CreateDate = postCategory.LastEditDate = DateTime.Now;
            _context.PostCategories.Add(postCategory);
        }

        public void Update(PostCategory postCategory)
        {
            postCategory.LastEditDate = DateTime.Now;
            _context.PostCategories.Update(postCategory);
        }

        public async Task Delete(int id)
        {
            var postCategory = await GetById(id);

            if (postCategory != null)
            {
                _context.PostCategories.Remove(postCategory);
            }
        }

        public async Task<bool> IsPostCategoryExists(PostCategory postCategory)
        {
            return postCategory.Id == 0
                ? await _context.PostCategories
                    .AnyAsync(x => x.Title == postCategory.Title)
                : await _context.PostCategories
                    .AnyAsync(x => x.Title == postCategory.Title && x.Id != postCategory.Id);
        }

        public async Task<int> Count()
        {
            return await _context.PostCategories.CountAsync();
        }
    }
}
