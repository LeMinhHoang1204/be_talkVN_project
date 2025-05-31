using TalkVN.Application;
using TalkVN.DataAccess;
using TalkVN.DataAccess.Data;
using TalkVN.Domain.Common;
using TalkVN.Infrastructure;
using TalkVN.Infrastructure.SignalR;
using TalkVN.Infrastructure.Validations;
using TalkVN.WebAPI;

using FluentValidation;
using FluentValidation.AspNetCore;

using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

using TalkVN.Domain.Identity;
using Microsoft.Extensions.Logging;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.UseUrls("http://0.0.0.0:8080");
DotNetEnv.Env.Load();
builder.Configuration
    .AddEnvironmentVariables();

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = GoogleDefaults.AuthenticationScheme;
})
    .AddCookie(options =>
    {
        options.Cookie.SameSite = SameSiteMode.Lax;
        options.Cookie.SecurePolicy = CookieSecurePolicy.None; // Không sử dụng cookie bảo mật trong môi trường phát triển
    })
    .AddGoogle(GoogleDefaults.AuthenticationScheme, options =>
    {
        // options.ClientId = builder.Configuration.GetSection("GoogleKeys:ClientId").Value;
        // options.ClientSecret = builder.Configuration.GetSection("GoogleKeys:ClientSecret").Value;

        options.ClientId = Environment.GetEnvironmentVariable("GoogleKeys__ClientId");
        options.ClientSecret = Environment.GetEnvironmentVariable("GoogleKeys__ClientSecret");
        options.CallbackPath = new PathString("/api/User/login-google/callback");
    });

// Add services to the container.
var env = builder.Environment;
builder.Configuration
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: false, reloadOnChange: true);

ValidatorOptions.Global.DefaultRuleLevelCascadeMode = CascadeMode.Stop;
builder.Services
    .AddFluentValidationAutoValidation()
    .AddFluentValidationClientsideAdapters()
    .AddControllers(config => config.Filters.Add(typeof(ValidateModelAttribute)))
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new UnixTimestampConverter());
    }); ;
// Learn more about configuring Swagger/CoOpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDataAccessService(builder.Configuration).AddApplicationServices();
builder.AddInfrastructure().AddWebAPI();

var connectionString = builder.Environment.IsProduction()
    ? builder.Configuration.GetConnectionString("AWSConnection")
    : builder.Configuration.GetConnectionString("DefaultConnection");
Console.WriteLine("Connection String: " + connectionString);
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseMySql(connectionString, new MySqlServerVersion(new Version(8, 0, 39))));

// // Configure the DbContext with the connection string
// builder.Services.AddDbContext<ApplicationDbContext>(options =>
//     options.UseMySql(builder.Configuration.GetConnectionString("DefaultConnection"),
//         new MySqlServerVersion(new Version(8, 0, 39))));

//CORS
const string corsPolicyName = "AllowVercelFrontend";
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: corsPolicyName, policy =>
    {
        policy.WithOrigins("http://localhost:3000","https://talkvn.vercel.app")
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});


builder.Logging.AddConsole();

var app = builder.Build();
app.UseCors(corsPolicyName);
// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI();

// Migrate Database
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var logger = services.GetRequiredService<ILogger<Program>>(); // Lấy ILogger
    try
    {
        logger.LogInformation("Attempting to migrate and seed database...");

        var context = services.GetRequiredService<ApplicationDbContext>();
        var roleManager = services.GetRequiredService<RoleManager<ApplicationRole>>();
        var userManager = services.GetRequiredService<UserManager<UserApplication>>();

        // 1. Áp dụng các migration đang chờ (nếu có)
        await AutomatedMigration.MigrateAsync(scope.ServiceProvider);
        logger.LogInformation("Database migration completed (if any pending).");

        // 2. Chạy tất cả các hàm seeding
        await DbContextSeed.SeedDatabaseAsync(userManager, roleManager);
        await ApplicationDbSeeder.SeedAllAsync(context, roleManager, logger); // Truyền logger vào

        logger.LogInformation("Database migrated and seeded successfully.");
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "AN ERROR OCCURRED WHILE MIGRATING OR SEEDING THE DATABASE.");
        // Bạn có thể muốn dừng ứng dụng ở đây nếu seeding là bắt buộc và thất bại
        // throw;
    }
}

app.UseHttpsRedirection();
app.AddInfrastuctureApplication();
app.UseAuthentication();
app.UseAuthorization();
app.AddSignalRHub();
// app.UseCors(corsPolicyBuilder => corsPolicyBuilder
//         .AllowAnyOrigin()
//         .AllowAnyMethod()
//         .AllowAnyHeader()
//     );
app.MapControllers();
Console.WriteLine(builder.Configuration["GoogleKeys:ClientId"]);

app.Run();

