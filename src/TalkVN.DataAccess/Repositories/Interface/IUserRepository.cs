using System.Linq.Expressions;

using TalkVN.Domain.Entities;
using TalkVN.Domain.Identity;

using Microsoft.EntityFrameworkCore.Query;

using TalkVN.Domain.Common;

namespace TalkVN.DataAccess.Repositories.Interface
{
    public interface IUserRepository
    {
        IQueryable<UserApplication> GetQuery();

        Task<UserApplication> GetFirstAsync(Expression<Func<UserApplication, bool>> predicate);

        Task<UserApplication> GetFirstOrDefaultAsync(Expression<Func<UserApplication, bool>> predicate);
        Task<List<UserApplication>> GetAllAsync(Expression<Func<UserApplication, bool>> predicate);

        Task<List<UserApplication>> GetAllAsync(Expression<Func<UserApplication, bool>> predicate, IEnumerable<Expression<Func<UserApplication, BaseEntity>>> includes);
        Task<List<UserApplication>> GetAllAsync(Expression<Func<UserApplication, bool>> predicate, Func<IQueryable<UserApplication>, IIncludableQueryable<UserApplication, object>> includeQuery);
        Task<List<UserApplication>> GetAllAsync(Expression<Func<UserApplication, bool>> predicate, Func<IQueryable<UserApplication>, IOrderedQueryable<UserApplication>> sort);

        Task<PaginationResponse<UserApplication>> GetAllAsync(
            Expression<Func<UserApplication, bool>> predicate,
            Func<IQueryable<UserApplication>, IOrderedQueryable<UserApplication>> sort,
            int pageIndex,
            int pageSize);

        Task<UserApplication> UpdateAsync(UserApplication entity);

        Task<List<UserApplication>> UpdateRangeAsync(List<UserApplication> entities);

        Task<UserApplication> DeleteAsync(UserApplication entity);

        Task<bool> AnyAsync(Expression<Func<UserApplication, bool>> predicate);

    }
}
