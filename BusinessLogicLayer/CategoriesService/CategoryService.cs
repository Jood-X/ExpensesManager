using AutoMapper;
using ExpenseManager.BusinessLayer.CategoriesService.CategoriesDTO;
using ExpenseManager.DataAccessLayer.Entities;
using ExpenseManager.DataAccessLayer.Interfaces.CategoriesRepository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Microsoft.Extensions.Configuration;
using System.Security.Claims;
using System.Text;

namespace ExpenseManager.BusinessLayer.CategoriesService
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly IMapper _mapper;
        private readonly IConfiguration _config;
        private readonly IHttpContextAccessor _httpContextAccessor; 

        public CategoryService(ICategoryRepository categoryRepository, IMapper mapper, IConfiguration config, IHttpContextAccessor httpContextAccessor)
        {
            _categoryRepository = categoryRepository ?? throw new ArgumentNullException(nameof(categoryRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _config = config ?? throw new ArgumentNullException(nameof(config));
            _httpContextAccessor = httpContextAccessor;
        }

        private string GetUserIdOrThrow()
        {
            var userId = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value; 
            if (string.IsNullOrEmpty(userId))
                throw new UnauthorizedAccessException("User ID is missing from token.");
            return userId;
        }

        public async Task<CategoryPagingDTO> GetAllCategoriesAsync(string? searchTerm, int page = 1)
        {
            var pageSizeSetting = _config["AppSettings:PageSize"];
            if (string.IsNullOrEmpty(pageSizeSetting))
            {
                throw new InvalidOperationException("Page size configuration is missing.");
            }

            if (!int.TryParse(pageSizeSetting, out var pageResult))
            {
                throw new InvalidOperationException("Page size configuration is invalid.");
            }
            var totalUsersCount = await _categoryRepository.GetAllCategoriesCount();
            var pageCount = (int)Math.Ceiling(totalUsersCount / (double)pageResult);

            var userId = GetUserIdOrThrow();
            var allCategories = await _categoryRepository.GetAllCategoriesAsync(); 

            var categories = allCategories
                .Where(w => w.CreateBy.ToString() == userId);

            if (searchTerm != null)
            {
                searchTerm = searchTerm.ToLower();
                categories = categories.Where(u => u.Name.ToLower().Contains(searchTerm));
            }
            categories = categories
                .Skip((page - 1) * pageResult)
                .Take(pageResult);

            var response = new CategoryPagingDTO
            {
                Categories = categories.Select(t => _mapper.Map<CategoryDTO>(t)).ToList(),
                Pages = pageCount,
                CurrentPage = page
            };

            return response;
        }

        public async Task<CategoryDTO> GetCategoryByIdAsync(int categoryId)
        {
            var userId = GetUserIdOrThrow();
            var category = await _categoryRepository.GetCategoryByIdAsync(categoryId);

            if (category == null || category.CreateBy.ToString() != userId)
            {
                throw new InvalidOperationException("Category not found.");
            }

            return _mapper.Map<CategoryDTO>(category);
        }
        public async Task<bool> CreateCategoryAsync(CreateCategoryDTO newCategory)
        {
            var userId = GetUserIdOrThrow();

            if (newCategory == null)
            {
                throw new ArgumentNullException(nameof(newCategory), "Category cannot be null");
            }
            var category = _mapper.Map<Category>(newCategory);
            category.CreateDate = DateTime.Now;
            category.CreateBy = int.Parse(userId);

            await _categoryRepository.Add(category);
            return true;
        }

        public async Task<bool> UpdateCategoryAsync(int id, UpdateCategoryDTO updatedCategory)
        {
            var userId = GetUserIdOrThrow();

            var category = await _categoryRepository.GetCategoryByIdAsync(id);
            if (category == null || category.CreateBy.ToString() != userId)
            {
                throw new InvalidOperationException("Category not found or you do not have permission to update this category.");
            }
            category.UpdateDate = DateTime.Now;
            _mapper.Map(updatedCategory, category);
            await _categoryRepository.Update(category);
            return true;
        }
        public async Task<bool> DeleteCategoryAsync(int categoryId)
        {
            var userId = GetUserIdOrThrow();

            var category = await _categoryRepository.GetCategoryByIdAsync(categoryId);
            if (category == null || category.CreateBy.ToString() != userId)
            {
                throw new InvalidOperationException("Category not found or you do not have permission to delete this category.");
            }
            await _categoryRepository.Delete(category);
            return true;
        }

        public async Task<FileContentResult> GetCategoriesReportAsync()
        {
            string[] columnNames = { "Id", "Name", "Type", "Limit", "Color", "CreateBy", "CreateDate", "UpdateBy", "UpdateDate"};
            var categories = await _categoryRepository.GetAllCategoriesAsync();
            if (categories == null || !categories.Any())
            {
                throw new InvalidOperationException("No categories found for the report.");
            }

            string csv = string.Empty;
            foreach (string columnName in columnNames)
            {
                csv += $"{columnName},";
            }
            csv += "\r\n";

            foreach (var category in categories)
            {
                csv += $"{category.Id},{category.Name},{category.Type},{category.Limit},{category.Color},{category.CreateByNavigation?.Name},{category.CreateDate},{category.UpdateByNavigation?.Name},{category.UpdateDate}\r\n";
            }
            byte[] bytes = Encoding.ASCII.GetBytes(csv);
            return new FileContentResult(bytes, "text/csv")
            {
                FileDownloadName = "CategoriesReport.csv"
            };
        }
    }
}
