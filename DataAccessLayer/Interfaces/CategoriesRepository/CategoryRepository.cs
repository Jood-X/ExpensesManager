using ExpenseManager.DataAccessLayer.Data;
using ExpenseManager.DataAccessLayer.Entities;
using ExpenseManager.DataAccessLayer.Interfaces.GenericRepository;
using Microsoft.EntityFrameworkCore;

namespace ExpenseManager.DataAccessLayer.Interfaces.CategoriesRepository
{
    public class CategoryRepository : GenericRepo<Category>, ICategoryRepository
    {
        private readonly WalletManagerDbContext _context;
        public CategoryRepository(WalletManagerDbContext context) : base(context) 
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public Task<int> GetAllCategoriesCount()
        {
            return base.Count();
        }
        public async Task<IEnumerable<Category>> GetAllCategoriesAsync()
        {
            return await _context.Categories
                .Include(c => c.CreateByNavigation)
                .Include(c => c.UpdateByNavigation)
                .ToListAsync();
        }

        public async Task<Category?> GetCategoryByIdAsync(int categoryId)
        {
            return await _context.Categories
                .Include(c => c.CreateByNavigation)
                .FirstOrDefaultAsync(c => c.Id == categoryId);
        }
    }
}
