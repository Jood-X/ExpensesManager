using ExpenseManager.BusinessLayer.UserService;
using ExpenseManager.BusinessLayer.UserService.UserDTO;
using ExpenseManager.DataAccessLayer.Entities;
using Microsoft.AspNetCore.Mvc;
using WebApplication2.Controllers;

namespace ExpenseManager.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ILogger<UsersController> _logger;

        public UsersController(IUserService userService, ILogger<UsersController> logger)
        {
            _userService = userService;
            _logger = logger;
            _logger.LogDebug("Nlog is integrated to UsersController");
        }

        [HttpGet]
        public async Task<ApiResponse<UsersPagingDTO>> GetAll(string? searchTerm, int page = 1)
        {
            try
            {
                var data = await _userService.GetAllUsersAsync(searchTerm, page);
                return ApiResponse<UsersPagingDTO>.SuccessResponse(data, "Users retrieved successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return ApiResponse<UsersPagingDTO>.ErrorResponse("An error occurred while retrieving the users", ex.Message); 
            }            
        }

        [HttpGet("details/{id}")]
        public async Task<ApiResponse<UsersDTO>> GetByID(int id)
        {
            try
            {
                var user = await _userService.GetUserByIdAsync(id);
                return ApiResponse<UsersDTO>.SuccessResponse(user, "User retreived successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return ApiResponse<UsersDTO>.ErrorResponse("An error occurred while retrieving the user", ex.Message);
            }
        }

        [HttpPost]
        public async Task<ApiResponse<UsersDTO>> Create(CreateUserDTO newUser)
        {
            try
            {
                var user = await _userService.CreateUserAsync(newUser);
                return ApiResponse<UsersDTO>.SuccessResponse(user, "New User Added Successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return ApiResponse<UsersDTO>.ErrorResponse("An error occurred while creating the user", ex.Message);
            }
        }

        [HttpPut("{id}")]
        public async Task<ApiResponse<string>> Update(int id, UpdateUserDTO updatedUser) 
        {
            try
            {
                var user = await _userService.UpdateUserAsync(updatedUser);
                return ApiResponse<string>.SuccessResponse("User Updated Successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return ApiResponse<string>.ErrorResponse("An error occurred while updating the user", ex.Message);
            }
        }

        [HttpDelete("{id}")]
        public async Task<ApiResponse<string>> Delete(int id) 
        {
            try
            {
                var user = await _userService.DeleteUserAsync(id);
                return ApiResponse<string>.SuccessResponse("User Deleted Successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return ApiResponse<string>.ErrorResponse("An error occurred while updating the user", ex.Message);
            }
        }

        [HttpGet("report")]
        public async Task<IActionResult> GetUsersReport()
        {
            try
            {
                var report = await _userService.GetUsersReportAsync();
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
