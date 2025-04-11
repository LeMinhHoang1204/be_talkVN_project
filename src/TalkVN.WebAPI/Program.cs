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

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

using TalkVN.Domain.Identity;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.UseUrls("http://0.0.0.0:8080");

// Add services to the container.
var env = builder.Environment;
builder.Configuration
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: false, reloadOnChange: true);

DotNetEnv.Env.Load();
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
Console.WriteLine("Connection String: " + builder.Configuration.GetConnectionString("AWSConnection"));
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseMySql(connectionString, new MySqlServerVersion(new Version(8, 0, 39))));

// // Configure the DbContext with the connection string
// builder.Services.AddDbContext<ApplicationDbContext>(options =>
//     options.UseMySql(builder.Configuration.GetConnectionString("DefaultConnection"),
//         new MySqlServerVersion(new Version(8, 0, 39))));

// const string corsPolicyName = "AllowVercelFrontend";
// builder.Services.AddCors(options =>
// {
//     options.AddPolicy(name: corsPolicyName, policy =>
//     {
//         policy.WithOrigins("https://fetalkvnproject.vercel.app")
//             .AllowAnyHeader()
//             .AllowAnyMethod()
//             .AllowCredentials();
//     });
// });


builder.Logging.AddConsole();

var app = builder.Build();
// app.UseCors(corsPolicyName);
// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI();

// Migrate Database
using var scope = app.Services.CreateAsyncScope();
await AutomatedMigration.MigrateAsync(scope.ServiceProvider);

// Gọi seed sau khi migrate
var userManager = scope.ServiceProvider.GetRequiredService<UserManager<UserApplication>>();
var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<ApplicationRole>>();
await DbContextSeed.SeedDatabaseAsync(userManager, roleManager);

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

app.Run();

