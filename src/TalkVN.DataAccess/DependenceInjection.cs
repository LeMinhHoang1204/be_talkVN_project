using TalkVN.DataAccess.Repositories;
using TalkVN.DataAccess.Repositories.Interface;
using TalkVN.DataAccess.Repositories.Interface;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace TalkVN.DataAccess
{
    public static class DependenceInjection
    {
        public static IServiceCollection AddDataAccessService(this IServiceCollection services, IConfiguration configuration)
        {

            services.AddRepositories();
            return services;
        }
        private static void AddRepositories(this IServiceCollection services)
        {
            services
            .AddScoped(typeof(IBaseRepository<>), typeof(BaseRepository<>))
            .AddScoped<IRepositoryFactory, RepositoryFactory>()
            .AddScoped<IUserRepository, UserRepository>()
            .AddScoped<IConversationRepository, ConversationRepository>()
            .AddScoped<IGroupInviteRepository, GroupInviteRepository>()
            .AddScoped<IGroupRepository, GroupRepository>()
            .AddScoped<IUserInteractionRepository, UserInteractionRepository>()
            .AddScoped<IUserFollowerRepository, UserFollowerRepository>();
            //TODO: khai bao repo trong day

        }

    }
}
