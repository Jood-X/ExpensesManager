using ExpenseManager.BusinessLayer.CategoriesService.CategoriesDTO;
using ExpenseManager.DataAccessLayer.Entities;
using Microsoft.AspNetCore.Mvc;

namespace ExpenseManager.BusinessLayer.CategoriesService
{
    public interface ICategoryService
    {
        Task<CategoryPagingDTO> GetAllCategoriesAsync(string? searchTerm, int page = 1);
        Task<CategoryDTO> GetCategoryByIdAsync(int categoryId);
        Task<bool> CreateCategoryAsync(CreateCategoryDTO newCategory);
        Task<bool> UpdateCategoryAsync(UpdateCategoryDTO updatedCategory);
        Task<bool> DeleteCategoryAsync(int categoryId);
        Task<FileContentResult> GetCategoriesReportAsync();
        Task<IEnumerable<CategoryUIDTO>> GetAllCategoriesAsync();
    }
}
