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

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.UseUrls("http://0.0.0.0:8080", "http://0.0.0.0:8081");

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
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services
    .AddDataAccessService(builder.Configuration)
    .AddApplicationServices();
builder
    .AddInfrastructure()
    .AddWebAPI()
    ;
var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI();

// Migrate Database
using var scope = app.Services.CreateAsyncScope();
await AutomatedMigration.MigrateAsync(scope.ServiceProvider);

app.UseHttpsRedirection();
app.AddInfrastuctureApplication();
app.UseAuthentication();
app.UseAuthorization();
app.AddSignalRHub();
app.UseCors(corsPolicyBuilder => corsPolicyBuilder
        .AllowAnyOrigin()
        .AllowAnyMethod()
        .AllowAnyHeader()
    );
app.MapControllers();

app.Run();

