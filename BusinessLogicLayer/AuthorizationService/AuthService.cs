using ExpenseManager.BusinessLayer.AuthorizationService.AuthDTO;
using ExpenseManager.BusinessLayer.EmailService;
using ExpenseManager.BusinessLayer.UserService.UserDTO;
using ExpenseManager.DataAccessLayer.Data;
using ExpenseManager.DataAccessLayer.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace ExpenseManager.BusinessLayer.AuthorizationService
{
    public class AuthService (WalletManagerDbContext context, IConfiguration configuration, IEmailSender emailSender) : IAuthService
    {
        public async Task<TokenResponseDTO?> LoginAsync(UserLoginDTO request)
        {
            var user = await context.Users.FirstOrDefaultAsync(u => u.Email == request.Email);
            if (user is null)
            {
                throw new Exception("User not found");
            }
            var result = new PasswordHasher<User>().VerifyHashedPassword(user, user.Password, request.Password);
            if (result == PasswordVerificationResult.Failed)
            {
                throw new Exception("Error with password");
            }

            if (user.IsEmailConfirmed == 0 || user.IsEmailConfirmed == null)
                throw new Exception("Email is not confirmed");

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
            if(await context.Users.AnyAsync(u => u.Email == request.Email))
            {
                return null;
            }
            var user = new User();
            var hashedPassword = new PasswordHasher<User>().HashPassword(user, request.Password);
            user.Name = request.Name;
            user.Email = request.Email;
            user.Password = hashedPassword;
            user.CreateDate = DateTime.Now;
            user.IsEmailConfirmed = 0;
            var code = new Random().Next(100000, 999999).ToString();
            user.EmailConfirmationCode = code;
            context.Users.Add(user);
            await context.SaveChangesAsync();

            var message = new Message(new string[] { user.Email }, "Expense Manager App", $"Welcome to expense manager app, email verification code: {code}");
            emailSender.SendEmail(message);
            return user;
        }
        public async Task<string> ConfirmEmail(ConfirmCodeDTO dto)
        {
            var user = await context.Users.FirstOrDefaultAsync(u => u.Email == dto.Email);
            if (user == null)
                throw new Exception ("User not found");

            if (user.IsEmailConfirmed==1)
                throw new Exception("Email is already confirmed");

            if (user.EmailConfirmationCode != dto.Code)
                throw new Exception("Invalid confirmation code");

            user.IsEmailConfirmed = 1;
            user.EmailConfirmationCode = null;

            await context.SaveChangesAsync();

            return "Email confirmed successfully";
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
            user.RefreshTokenExpiryTime = DateTime.Now.AddDays(7);
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
