using ExpenseManager.DataAccessLayer.Entities;
using ExpenseManager.DataAccessLayer.Interfaces.GenericRepository;

namespace ExpenseManager.DataAccessLayer.Interfaces.CategoriesRepository
{
    public interface ICategoryRepository: IGenericRepo<Category>
    {
        Task<int> GetAllCategoriesCount();
        Task<IEnumerable<Category>> GetAllCategoriesAsync();
        Task<Category?> GetCategoryByIdAsync(int categoryId);
    }

}
