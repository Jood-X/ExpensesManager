using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Configuration;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using ExpenseManager.DataAccessLayer.Data;
using ExpenseManager.DataAccessLayer.Entities;
using ExpenseManager.BusinessLayer.AuthorizationService.AuthDTO;
using ExpenseManager.BusinessLayer.UserService.UserDTO;

namespace ExpenseManager.BusinessLayer.AuthorizationService
{
    public class AuthService (WalletManagerDbContext context, IConfiguration configuration): IAuthService
    {
        public async Task<TokenResponseDTO?> LoginAsync(UserLoginDTO request)
        {
            var user = await context.Users.FirstOrDefaultAsync(u => u.Email == request.Email);
            if (user is null)
            {
                return null;
            }
            var result = new PasswordHasher<User>().VerifyHashedPassword(user, user.Password, request.Password);
            if (result == PasswordVerificationResult.Failed)
            {
                return null;
            }
            return await CreateTokenResponse(user);
        }

        private async Task<TokenResponseDTO> CreateTokenResponse(User user)
        {
            return new TokenResponseDTO
            {
                AccessToken = CreateToken(user),
                RefreshToken = await GenerateAndSaveRefreshTokenAsync(user),
            };
        }

        public async Task<User?> RegisterAsync(CreateUserDTO request)
        {
            if(await context.Users.AnyAsync(u => u.Name == request.Email))
            {
                return null;
            }
            var user = new User();
            var hashedPassword = new PasswordHasher<User>().HashPassword(user, request.Password);
            user.Name = request.Name;
            user.Email = request.Email;
            user.Password = hashedPassword;
            user.CreateDate = DateTime.UtcNow;
            context.Users.Add(user);
            await context.SaveChangesAsync();
            return user;
        }

        public async Task<TokenResponseDTO?> RefreshTokensAsync(RefreshTokenRequestDTO request)
        {
            var user = await ValidateRefreshTokenAsync(request.id, request.RefreshToken);
            if (user is null)
            {
                return null;
            }
            return await CreateTokenResponse(user);
        }

        private async Task<User?> ValidateRefreshTokenAsync(int id, string refreshToken)
        {
            var user = await context.Users.FirstOrDefaultAsync(u => u.Id == id);
            if (user is null || user.RefreshToken != refreshToken || user.RefreshTokenExpiryTime <= DateTime.UtcNow)
            {
                return null;
            }
            return user;
        }
        private string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }

        private async Task<string> GenerateAndSaveRefreshTokenAsync(User user)
        {
            var refreshToken = GenerateRefreshToken();
            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
            await context.SaveChangesAsync();
            return refreshToken;
        }

        private string CreateToken(User user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Email),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())
            };
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["AppSettings:Token"]!));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512);
            var tokenDescriptor = new JwtSecurityToken(
                issuer: configuration["AppSettings:Issuer"],
                audience: configuration["AppSettings:Audience"],
                claims: claims,
                expires: DateTime.Now.AddDays(1),
                signingCredentials: creds
                );
            return new JwtSecurityTokenHandler().WriteToken(tokenDescriptor);
        }
    }
}
