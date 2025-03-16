using TalkVN.Application.Helpers;
using TalkVN.DataAccess.Data;
using TalkVN.Domain.Identity;
using TalkVN.Infrastructure.Caching;
using TalkVN.Infrastructure.CloudinaryConfigurations;
using TalkVN.Infrastructure.EntityFrameworkCore;
using TalkVN.Infrastructure.Logging;
using TalkVN.Infrastructure.Middleware;
using TalkVN.Infrastructure.Services;
using TalkVN.Infrastructure.SignalR;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace TalkVN.Infrastructure
{
    public static class DependencyInjection
    {
        public static WebApplicationBuilder AddInfrastructure(this WebApplicationBuilder builder)
        {
            builder
            .AddEntityFramewordCore()
            //.AddAppAuthorization()
            .AddCaching()
            .AddSignalRRegistration();


            // Services
            builder.Services
             .AddInfrastructureService().AddAuthorization();

            // Host
            builder.Host.AddHostSerilogConfiguration();

            // Validation


            return builder;

        }
        public static IServiceCollection AddInfrastructureService(this IServiceCollection services)
        {
            services.AddIdentity();
            services.AddCloudinary();
            services.AddSingleton<IClaimService, ClaimService>();
            services.AddSingleton<ITokenService, TokenService>();
            services.AddSingleton<ICloudinaryService, CloudinaryService>();
            return services;
        }
        private static void AddIdentity(this IServiceCollection services)
        {
            services.AddIdentity<UserApplication, ApplicationRole>(options =>
            {
                options.SignIn.RequireConfirmedAccount = true;
            })
                .AddEntityFrameworkStores<ApplicationDbContext>();
            services.Configure<IdentityOptions>(options =>
            {
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireNonAlphanumeric = true;
                options.Password.RequireUppercase = true;
                options.Password.RequiredLength = 6;
                options.Password.RequiredUniqueChars = 1;

                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromDays(30);
                options.Lockout.MaxFailedAccessAttempts = 5;
                options.Lockout.AllowedForNewUsers = true;

                options.User.RequireUniqueEmail = true;
            });
        }
        public static IApplicationBuilder AddInfrastuctureApplication(this IApplicationBuilder app)
        {
            app.UseMiddleware<GlobalExceptionHandlerMiddleware>();
            return app;
        }

    }
}
