using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ExpenseManager.BusinessLayer.AuthorizationService.AuthDTO;
using ExpenseManager.BusinessLayer.AuthorizationService;
using ExpenseManager.BusinessLayer.UserService.UserDTO;
using ExpenseManager.BusinessLayer.EmailService;
using ExpenseManager.BusinessLayer.UserService;

namespace ExpenseManager.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController(IAuthService authService, IUserService userService) : ControllerBase
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
        [HttpPost("confirmemail")]
        public async Task<ApiResponse<string>> ConfirmEmail([FromBody] ConfirmCodeDTO dto)
        {
            try
            {
                var result = await authService.ConfirmEmail(dto);
                return ApiResponse<string>.SuccessResponse(result);
            }
            catch (Exception ex)
            {
                return ApiResponse<string>.ErrorResponse();
            }
        }

        [HttpPost("login")]
        public async Task<ApiResponse<TokenResponseDTO>> Login(UserLoginDTO request)
        {
            try
            {
                var result = await authService.LoginAsync(request);
                return ApiResponse<TokenResponseDTO>.SuccessResponse(result);
            }
            catch (Exception ex)
            {
                return ApiResponse<TokenResponseDTO>.ErrorResponse(ex.Message);
            }
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
