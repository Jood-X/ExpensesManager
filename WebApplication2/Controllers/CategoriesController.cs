using AutoMapper;
using ExpenseManager.Api;
using ExpenseManager.Api.Controllers;
using ExpenseManager.BusinessLayer.CategoriesService;
using ExpenseManager.BusinessLayer.CategoriesService.CategoriesDTO;
using ExpenseManager.BusinessLayer.UserService.UserDTO;
using ExpenseManager.DataAccessLayer.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace WebApplication2.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CategoriesController : ControllerBase
    {
        private readonly ICategoryService _categoryService;
        private readonly ILogger<CategoriesController> _logger;

        public CategoriesController(ICategoryService categoryService, ILogger<CategoriesController> logger)
        {
            _categoryService = categoryService ?? throw new ArgumentNullException(nameof(categoryService));
            _logger = logger;
            _logger.LogDebug("Nlog is integrated to CategoriesController");
        }

        [HttpGet]
        public async Task<ApiResponse<CategoryPagingDTO>> GetAll(string? searchTerm, int page = 1)
        {
            try
            {
                var response = await _categoryService.GetAllCategoriesAsync(searchTerm, page);
                return ApiResponse<CategoryPagingDTO>.SuccessResponse(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return ApiResponse<CategoryPagingDTO>.ErrorResponse("An error occurred while retrieving users", ex.Message);
            }
        }

        [HttpGet("MyCategories")]
        public async Task<ApiResponse<IEnumerable<CategoryUIDTO>>> GetAll()
        {
            try
            {
                var response = await _categoryService.GetAllCategoriesAsync();
                return ApiResponse<IEnumerable<CategoryUIDTO>>.SuccessResponse(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return ApiResponse<IEnumerable<CategoryUIDTO>>.ErrorResponse("An error occurred while retrieving users", ex.Message);
            }
        }

        [HttpGet("{id}")]
        public async Task<ApiResponse<CategoryDTO>> GetByID(int id)
        {
            try
            {
                var category = await _categoryService.GetCategoryByIdAsync(id);
                return ApiResponse<CategoryDTO>.SuccessResponse(category, "Category retrieved successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return ApiResponse<CategoryDTO>.ErrorResponse("An error occurred while retrieving the category", ex.Message);
            }
        }

        [HttpPost]
        public async Task<ApiResponse<string>> Create(CreateCategoryDTO newCategory)
        {
            try
            {
                await _categoryService.CreateCategoryAsync(newCategory);
                return ApiResponse<string>.SuccessResponse("Category Added Successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return ApiResponse<string>.ErrorResponse("An error occurred while creating the category", ex.Message);
            }
        }

        [HttpPut]
        public async Task<ApiResponse<string>> Update(UpdateCategoryDTO updatedCategory)
        {
            try
            {
                await _categoryService.UpdateCategoryAsync(updatedCategory);
                return ApiResponse<string>.SuccessResponse("Category Updated Successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return ApiResponse<string>.ErrorResponse("An error occurred while updating the category", ex.Message);
            }
        }

        [HttpDelete("{id}")]
        public async Task<ApiResponse<string>> Delete(int id)
        {
            try
            {
                await _categoryService.DeleteCategoryAsync(id);
                return ApiResponse<string>.SuccessResponse("Category Deleted Successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return ApiResponse<string>.ErrorResponse("An error occurred while deleting the category", ex.Message);
            }
        }
        [HttpGet("report")]
        public async Task<IActionResult> GetCategoriesReport()
        {
            try
            {
                var report = await _categoryService.GetCategoriesReportAsync();
                return File(report.FileContents, report.ContentType, report.FileDownloadName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return BadRequest(new ApiResponse<string>(false, "An error occurred while generating the report", null, ex.Message));
            }
        }
    }
}
