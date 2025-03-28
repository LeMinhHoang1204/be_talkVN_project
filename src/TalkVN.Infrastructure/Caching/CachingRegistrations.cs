using TalkVN.Application.Services.Caching;

using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace TalkVN.Infrastructure.Caching
{
    public static class CachingRegistrations
    {
        public static WebApplicationBuilder AddCaching(this WebApplicationBuilder builder)
        {
            builder.Services
                .AddMemoryCache()
                .AddSingleton<ICachingService, MemoryCacheService>();
            return builder;
        }
    }

}
