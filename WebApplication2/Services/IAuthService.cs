using WebApplication2.DTO;
using WebApplication2.Models;

namespace WebApplication2.Services
{
    public interface IAuthService
    {
        Task<User?> RegisterAsync(CreateUserDTO request);
        Task<string?> LoginAsync(UserLoginDTO request);
    }
}
