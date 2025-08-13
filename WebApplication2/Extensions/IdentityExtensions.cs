using ExpenseManager.DataAccessLayer.Data;
using ExpenseManager.DataAccessLayer.Entities;
using Microsoft.AspNetCore.Identity;

namespace ExpenseManager.Api.Extensions
{
    public static class IdentityExtensions
    {
        public static void AddIdentityHandlerAndStores(this IServiceCollection services)
        {
            services.AddIdentityApiEndpoints<User>()
                    .AddEntityFrameworkStores<WalletManagerDbContext>();
        }

        public static void ConfigureIdentityOptions(this IServiceCollection services)
        {
            services.Configure<IdentityOptions>(options =>
            {
                options.Password.RequireDigit = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireUppercase = false;
                options.User.RequireUniqueEmail = true;
            });
        }
    }
}
