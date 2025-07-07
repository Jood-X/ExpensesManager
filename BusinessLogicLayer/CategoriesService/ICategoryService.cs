using ExpenseManager.BusinessLayer.CategoriesService.CategoriesDTO;
using ExpenseManager.DataAccessLayer.Entities;

namespace ExpenseManager.BusinessLayer.CategoriesService
{
    public interface ICategoryService
    {
        Task<CategoryPagingDTO> GetAllCategoriesAsync(string? searchTerm, int page = 1);
        Task<CategoryDTO> GetCategoryByIdAsync(int categoryId);
        Task<bool> CreateCategoryAsync(CreateCategoryDTO newCategory);
        Task<bool> UpdateCategoryAsync(int id, UpdateCategoryDTO updatedCategory);
        Task<bool> DeleteCategoryAsync(int categoryId);
        Task<IEnumerable<Category>> GetCategoriesByWalletIdAsync(int walletId);
    }
}
