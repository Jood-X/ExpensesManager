using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using WebApplication2.Data;
using WebApplication2.DTO;
using WebApplication2.Models;

namespace WebApplication2.Services
{
    public class AuthService (WalletManagerDbContext context, IConfiguration configuration): IAuthService
    {
        public async Task<string?> LoginAsync(UserLoginDTO request)
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
            return CreateToken(user);
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

        private string CreateToken(User user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Email),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())
            };
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration.GetValue<string>("AppSettings:Token")!));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512);
            var tokenDescriptor = new JwtSecurityToken(issuer: configuration.GetValue<string>("AppSettings:Issuer"),
                audience: configuration.GetValue<string>("AppSettings:Audience"),
                claims: claims,
                expires: DateTime.Now.AddDays(1),
                signingCredentials: creds
                );

            return new JwtSecurityTokenHandler().WriteToken(tokenDescriptor);
        }
    }
}
