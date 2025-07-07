using ExpenseManager.DataAccessLayer.Entities;
using ExpenseManager.BusinessLayer.UserService.UserDTO;

namespace ExpenseManager.BusinessLayer.UserService
{
    public interface IUserService
    {
        Task<UsersPagingDTO> GetAllUsersAsync(string? searchTerm, int page = 1);
        Task<UsersDTO> GetUserByIdAsync(int id);
        Task<UsersDTO> CreateUserAsync(CreateUserDTO user);
        Task<bool> UpdateUserAsync(UpdateUserDTO user);
        Task<bool> DeleteUserAsync(int userId);
    }
}
