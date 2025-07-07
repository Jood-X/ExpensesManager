using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ExpenseManager.BusinessLayer.AuthorizationService.AuthDTO;
using ExpenseManager.BusinessLayer.AuthorizationService;
using ExpenseManager.BusinessLayer.UserService.UserDTO;

namespace ExpenseManager.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController(IAuthService authService) : ControllerBase
    {
        [HttpPost("register")]
        public async Task<ApiResponse<string>> Register(CreateUserDTO request)
        {
            var user = await authService.RegisterAsync(request);
            if (user is null)
            {
                return ApiResponse<string>.ErrorResponse("Email already exists");
            }
            string message = $"User {user.Name} Added Successfully";
            return ApiResponse<string>.SuccessResponse(message);
        }

        [HttpPost("login")]
        public async Task<ApiResponse<TokenResponseDTO>> Login(UserLoginDTO request)
        {
            var result = await authService.LoginAsync(request);
            if (result is null)
            {
                return ApiResponse<TokenResponseDTO>.ErrorResponse("Invalid email or password");
            }
            return ApiResponse<TokenResponseDTO>.SuccessResponse(result);
        }

        [HttpPost("refresh-token")]
        public async Task<ApiResponse<TokenResponseDTO>> RefreshToken(RefreshTokenRequestDTO request)
        {
            var result = await authService.RefreshTokensAsync(request);
            if (result is null || result.AccessToken is null || result.RefreshToken is null)
            {
                return ApiResponse<TokenResponseDTO>.ErrorResponse("Invalid refresh token", "Unauthorized");
            }
            return ApiResponse<TokenResponseDTO>.SuccessResponse(result);
        }

        [Authorize]
        [HttpGet]
        public IActionResult AuthenticationOnlyEndpoint()
        {
            return Ok("You are authenticated!");
        }
    }
}
