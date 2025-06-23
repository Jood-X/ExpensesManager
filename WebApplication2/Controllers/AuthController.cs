using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using WebApplication2.DTO;
using WebApplication2.Models;
using WebApplication2.Services;

namespace WebApplication2.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController(IAuthService authService) : ControllerBase
    {
        [HttpPost("register")]
        public async Task<ActionResult<string>> Register(CreateUserDTO request)
        {
            var user = await authService.RegisterAsync(request);
            if (user is null)
            {
                return BadRequest("Email already exists");
            }
            string message = $"User {user.Name} Added Successfully";
            return message;
        }

        [HttpPost("login")]
        public async Task<ActionResult<string>> Login(UserLoginDTO request)
        {
            var token = await authService.LoginAsync(request);
            if (token is null)
            {
                return BadRequest("Invalid email or password");
            }
            return Ok("Logged In Successfully");
        }

        [Authorize]
        [HttpGet]
        public IActionResult AuthenticationOnlyEndpoint()
        {
            return Ok("You are authenticated!");
        }
    }
}
