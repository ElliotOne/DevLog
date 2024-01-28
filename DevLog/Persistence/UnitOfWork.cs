using DevLog.Core.Domain;
using DevLog.Data;
using DevLog.Persistence.EfCoreRepositories;
using DevLog.Persistence.Repositories;
using Microsoft.AspNetCore.Identity;

namespace DevLog.Persistence
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;

        public UnitOfWork(
            ApplicationDbContext context,
            UserManager<User> userManager,
            SignInManager<User> signInManager
        )
        {
            _context = context;

            CertificateRepository = new CertificateRepository(_context);
            ContactRepository = new ContactRepository(_context);
            PostCategoryRepository = new PostCategoryRepository(_context);
            PostCommentRepository = new PostCommentRepository(_context);
            PostRepository = new PostRepository(_context);
            ProgressBarRepository = new ProgressBarRepository(_context);
            SettingRepository = new SettingRepository(_context);
            UserRepository = new UserRepository(userManager, signInManager);
        }

        public ICertificateRepository CertificateRepository { get; set; }
        public IContactRepository ContactRepository { get; set; }
        public IPostCategoryRepository PostCategoryRepository { get; set; }
        public IPostCommentRepository PostCommentRepository { get; set; }
        public IPostRepository PostRepository { get; set; }
        public IProgressBarRepository ProgressBarRepository { get; set; }
        public ISettingRepository SettingRepository { get; set; }
        public IUserRepository UserRepository { get; set; }

        public async Task Complete()
        {
            await _context.SaveChangesAsync();
        }
    }
}
