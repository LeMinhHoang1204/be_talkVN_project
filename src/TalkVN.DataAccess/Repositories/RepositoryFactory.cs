using TalkVN.DataAccess.Repositories.Interface;
using TalkVN.Domain.Entities;

using Microsoft.Extensions.DependencyInjection;

namespace TalkVN.DataAccess.Repositories
{
    public class RepositoryFactory : IRepositoryFactory
    {
        private readonly IServiceProvider _serviceProvider;

        public RepositoryFactory(IServiceProvider serviceProvider)
        {
            this._serviceProvider = serviceProvider;
        }

        public IBaseRepository<TEntity> GetRepository<TEntity>() where TEntity : BaseEntity
            => this._serviceProvider.GetRequiredService<IBaseRepository<TEntity>>();
    }
}
