using ExpenseManager.DataAccessLayer.Entities;
using ExpenseManager.BusinessLayer.UserService.UserDTO;
using ExpenseManager.BusinessLayer.AuthorizationService.AuthDTO;

namespace ExpenseManager.BusinessLayer.AuthorizationService
{
    public interface IAuthService
    {
        Task<User?> RegisterAsync(CreateUserDTO request);
        Task<TokenResponseDTO?> LoginAsync(UserLoginDTO request);

        Task<TokenResponseDTO?> RefreshTokensAsync(RefreshTokenRequestDTO request);
        Task<string> ConfirmEmail(ConfirmCodeDTO dto);
    }
}
