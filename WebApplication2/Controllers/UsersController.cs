using ExpenseManager.BusinessLayer.UserService;
using ExpenseManager.BusinessLayer.UserService.UserDTO;
using ExpenseManager.DataAccessLayer.Entities;
using Microsoft.AspNetCore.Mvc;

namespace ExpenseManager.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;
        public UsersController(IUserService userService)
        {
            _userService = userService;
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
                return ApiResponse<string>.ErrorResponse("An error occurred while updating the user", ex.Message);
            }
        }
    }
}
