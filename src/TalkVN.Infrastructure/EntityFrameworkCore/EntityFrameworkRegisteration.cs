using TalkVN.DataAccess.Data;
using TalkVN.DataAccess.Data.Interceptor;

using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace TalkVN.Infrastructure.EntityFrameworkCore
{
    public static class EntityFrameworkRegisteration
    {
        public static WebApplicationBuilder AddEntityFramewordCore(this WebApplicationBuilder builder)
        {
            var configuration = builder.Configuration;

            builder.Services.AddScoped<ContextSaveChangeInterceptor>();

            builder.Services.AddDbContext<ApplicationDbContext>((provider, options) =>
            {
                var connectionString = builder.Environment.IsProduction()
                    ? builder.Configuration.GetConnectionString("AWSConnection")
                    : builder.Configuration.GetConnectionString("DefaultConnection");

                options
                    .AddInterceptors(provider.GetRequiredService<ContextSaveChangeInterceptor>())
                    .UseMySql(connectionString, new MySqlServerVersion(new Version(8, 0, 39)),
                        opt =>
                        {
                            opt.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName);
                            opt.EnableRetryOnFailure();
                        });
            });

            builder.Services.AddScoped<DbContext>(sp => sp.GetRequiredService<ApplicationDbContext>());

            return builder;
        }

    }
}
