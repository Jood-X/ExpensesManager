using ExpenseManager.BusinessLayer.AuthorizationService;
using ExpenseManager.BusinessLayer.CategoriesService;
using ExpenseManager.BusinessLayer.JobService;
using ExpenseManager.BusinessLayer.RecurringsService;
using ExpenseManager.BusinessLayer.TransactionsService;
using ExpenseManager.BusinessLayer.UserService;
using ExpenseManager.BusinessLayer.WalletService;
using ExpenseManager.DataAccessLayer.Data;
using ExpenseManager.DataAccessLayer.Interfaces.CategoriesRepository;
using ExpenseManager.DataAccessLayer.Interfaces.GenericRepository;
using ExpenseManager.DataAccessLayer.Interfaces.RecurringsRepository;
using ExpenseManager.DataAccessLayer.Interfaces.TransactionsRepository;
using ExpenseManager.DataAccessLayer.Interfaces.UserRepository;
using ExpenseManager.DataAccessLayer.Interfaces.WalletRepository;
using Hangfire;
using HangfireBasicAuthenticationFilter;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using System.Text.Json.Serialization;
using NLog;
using NLog.Web;
using ExpenseManager.BusinessLayer.EmailService;

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
    builder.Services.AddScoped<IEmailSender, EmailSender>();


    builder.Services.AddHangfire(config =>
    {
        config.UseSimpleAssemblyNameTypeSerializer()
              .UseRecommendedSerializerSettings()
              .UseSqlServerStorage(builder.Configuration.GetConnectionString("DefaultConnection"));
    });
    builder.Services.AddHangfireServer();
    
    builder.Services.AddCors(options =>
    {
        options.AddPolicy("AllowAngularDev", policy =>
        {
            policy.WithOrigins("http://localhost:4200")
                  .AllowAnyMethod()
                  .AllowAnyHeader()
                  .AllowCredentials(); 
        });
    });

    builder.Services.AddSingleton(
        builder.Configuration.GetSection("EmailConfiguration")
        .Get<EmailConfiguration>());

    var app = builder.Build();

    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    //app.UseCors("AllowAngularDev");
    app.UseCors(options => options.WithOrigins("http://localhost:4200")
    .AllowAnyMethod()
    .AllowAnyHeader());

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

    app.Run();
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