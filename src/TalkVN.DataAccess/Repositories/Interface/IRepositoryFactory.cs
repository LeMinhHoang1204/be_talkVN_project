using TalkVN.Domain.Entities;

namespace TalkVN.DataAccess.Repositories.Interface
{
    public interface IRepositoryFactory
    {
        IBaseRepository<TEntity> GetRepository<TEntity>() where TEntity : BaseEntity;
    }

}
