using ExpenseManager.Api;
using ExpenseManager.BusinessLayer.AuthorizationService;
using ExpenseManager.BusinessLayer.CategoriesService;
using ExpenseManager.BusinessLayer.JobService;
using ExpenseManager.BusinessLayer.RecurringsService;
using ExpenseManager.BusinessLayer.TransactionsService;
using ExpenseManager.BusinessLayer.UserService;
using ExpenseManager.BusinessLayer.WalletService;
using ExpenseManager.DataAccessLayer.Data;
using ExpenseManager.DataAccessLayer.Entities;
using ExpenseManager.DataAccessLayer.Interfaces.CategoriesRepository;
using ExpenseManager.DataAccessLayer.Interfaces.GenericRepository;
using ExpenseManager.DataAccessLayer.Interfaces.RecurringsRepository;
using ExpenseManager.DataAccessLayer.Interfaces.TransactionsRepository;
using ExpenseManager.DataAccessLayer.Interfaces.UserRepository;
using ExpenseManager.DataAccessLayer.Interfaces.WalletRepository;
using Hangfire;
using HangfireBasicAuthenticationFilter;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using System.Text.Json.Serialization;
using NLog;
using NLog.Web;

var logger = LogManager.Setup().LoadConfigurationFromAppSettings().GetCurrentClassLogger();
logger.Debug("init main");

try
{
    var builder = WebApplication.CreateBuilder(args);

    // NLog: Setup NLog for Dependency injection
    builder.Logging.ClearProviders();
    builder.Host.UseNLog();

    builder.Services.AddControllers()
        .AddJsonOptions(x =>
            x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles); ;
    // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
    builder.Services.AddEndpointsApiExplorer();

    builder.Services.AddSwaggerGen(c =>
    {
        c.SwaggerDoc("v1", new OpenApiInfo
        {
            Title = "Wallet Manager API",
            Version = "v1"
        });

        c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
        {
            Name = "Authorization",
            Type = SecuritySchemeType.ApiKey,
            Scheme = "Bearer",
            BearerFormat = "JWT",
            In = ParameterLocation.Header,
            Description = "Enter 'Bearer' followed by your valid JWT token in the textbox below.\n\nExample: `Bearer eyJhbGciOi...`"
        });

        c.AddSecurityRequirement(new OpenApiSecurityRequirement
        {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
        });
    });
    builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

    builder.Services.AddDbContext<WalletManagerDbContext>(Options =>
        Options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

    builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = builder.Configuration.GetValue<string>("AppSettings:Issuer"),
                ValidAudience = builder.Configuration.GetValue<string>("AppSettings:Audience"),
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration.GetValue<string>("AppSettings:Token")!))
            };
        });

    builder.Services.AddScoped<ICategoryService, CategoryService>();
    builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
    builder.Services.AddTransient<IHttpContextAccessor, HttpContextAccessor>();
    builder.Services.AddScoped(typeof(IGenericRepo<>), typeof(GenericRepo<>));
    builder.Services.AddScoped<IRecurringRepository, RecurringRepository>();
    builder.Services.AddScoped<IRecurringsService, RecurringsService>();
    builder.Services.AddScoped<ITransactionRepository, TransactionRepository>();
    builder.Services.AddScoped<ITransactionService, TransactionService>();
    builder.Services.AddScoped<IUserRepository, UserRepository>();
    builder.Services.AddScoped<IUserService, UserService>();
    builder.Services.AddScoped<IWalletRepository, WalletRepository>();
    builder.Services.AddScoped<IWalletService, WalletService>();
    builder.Services.AddScoped<IAuthService, AuthService>();
    builder.Services.AddScoped<IJobService, JobService>();

    builder.Services.AddHangfire(config =>
    {
        config.UseSimpleAssemblyNameTypeSerializer()
              .UseRecommendedSerializerSettings()
              .UseSqlServerStorage(builder.Configuration.GetConnectionString("DefaultConnection"));
    });
    builder.Services.AddHangfireServer();

    //Services from Identity Core
    //builder.Services.AddIdentityApiEndpoints<IdentityUser>()
    //                .AddEntityFrameworkStores<WalletManagerDbContext>();

    //builder.Services.Configure<IdentityOptions>(options =>
    //{
    //    options.Password.RequireDigit = false;
    //    options.Password.RequireLowercase = false;
    //    options.Password.RequireUppercase = false;
    //    options.User.RequireUniqueEmail = true;
    //});

    var app = builder.Build();

    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    //app.UseHttpsRedirection();

    //#region Config. Cors
    //app.UseCors(options => 
    //    options.WithOrigins("")
    //       .AllowAnyMethod()
    //       .AllowAnyHeader());
    //#endregion


    app.UseAuthentication();

    app.UseAuthorization();

    app.UseHangfireDashboard("/hangfire/job-dashboard", new DashboardOptions
    {
        DashboardTitle = "Hangfire Job Dashboard",
        DisplayStorageConnectionString = false,
        Authorization = new[]
        {
        new HangfireCustomBasicAuthenticationFilter
        {
            User = "admin",
            Pass = "admin123"
        }
    }
    });

    app.MapControllers();

    // for angular
    //app.MapGroup("/api")
    //   .MapIdentityApi<IdentityUser>();
    //app.MapPost("/api/signup", async(
    //        UserManager<IdentityUser> userManager,
    //        [FromBody] UserRegistrationModel userRegistrationModel
    //    ) => 
    //    {
    //        IdentityUser user = new IdentityUser()
    //        {
    //            Email = userRegistrationModel.Email,
    //            UserName = userRegistrationModel.FullName,
    //        };
    //        var result = await userManager.CreateAsync(user, userRegistrationModel.Password);
    //        if(result.Succeeded)
    //        {
    //            return Results.Ok(new { message = "User registered successfully." });
    //        }
    //        else
    //        {
    //            return Results.BadRequest(new { errors = result.Errors.Select(e => e.Description) });
    //        }
    //    });


    app.Run();

    /*
    public class UserRegistrationModel
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;

        public string FullName { get; set; } = string.Empty;
    }
    */
}
catch (Exception ex)
{
    logger.Error(ex, "Stopped program because of exception");
    throw;
}
finally
{
    LogManager.Shutdown();
}