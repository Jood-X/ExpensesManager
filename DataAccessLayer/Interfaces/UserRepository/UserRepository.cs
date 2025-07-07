using AutoMapper;
using Microsoft.EntityFrameworkCore;
using ExpenseManager.DataAccessLayer.Data;
using ExpenseManager.DataAccessLayer.Entities;
using ExpenseManager.DataAccessLayer.Interfaces.GenericRepository;

namespace ExpenseManager.DataAccessLayer.Interfaces.UserRepository
{
    public class UserRepository : GenericRepo<User>, IUserRepository
    {

        private readonly WalletManagerDbContext _context;

        public UserRepository(WalletManagerDbContext context): base(context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public Task<int> GetAllUsersCount()
        {
            return base.Count() ;
        }

        public Task<IEnumerable<User>> GetAllUsersAsync(int pageResult, int page = 1)
        {
            var users = _context.Users
                .Include(u => u.UpdateByNavigation)
                .Skip((page - 1) * pageResult)
                .Take(pageResult);

            return Task.FromResult<IEnumerable<User>>(users);
        }

        public async Task<User?> GetUserByIdAsync(int id)
        {
            var user = await _context.Users
                .Include(u => u.UpdateByNavigation)
                .FirstOrDefaultAsync(u => u.Id == id);
            return user;
        }
    }
}
