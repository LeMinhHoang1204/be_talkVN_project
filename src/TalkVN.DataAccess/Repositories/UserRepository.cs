using System.Linq.Expressions;

using TalkVN.DataAccess.Data;
using TalkVN.DataAccess.Repositories.Interface;
using TalkVN.Domain.Entities;
using TalkVN.Domain.Exceptions;
using TalkVN.Domain.Identity;

using Microsoft.EntityFrameworkCore.Query;

using TalkVN.Domain.Common;

namespace TalkVN.DataAccess.Repositories
{
    public class UserRepository : IUserRepository
    {
        protected readonly ApplicationDbContext Context;
        protected readonly DbSet<UserApplication> DbSet;
        public UserRepository(ApplicationDbContext context)
        {
            Context = context;
            DbSet = context.Set<UserApplication>();
        }
        public IQueryable<UserApplication> GetQuery() => this.DbSet;

        public async Task<UserApplication> GetFirstAsync(Expression<Func<UserApplication, bool>> predicate)
        {
            var entity = await DbSet.Where(predicate).FirstOrDefaultAsync();
            return entity ?? throw new ResourceNotFoundException(typeof(UserApplication));
        }

        public async Task<UserApplication> GetFirstOrDefaultAsync(Expression<Func<UserApplication, bool>> predicate)
        {
            return await DbSet.Where(predicate).FirstOrDefaultAsync();
        }
        public async Task<List<UserApplication>> GetAllAsync(Expression<Func<UserApplication, bool>> predicate)
        {
            return await DbSet.Where(predicate).ToListAsync();
        }

        public async Task<List<UserApplication>> GetAllAsync(Expression<Func<UserApplication, bool>> predicate, IEnumerable<Expression<Func<UserApplication, BaseEntity>>> includes)
        {
            var query = DbSet.Where(predicate);

            foreach (var include in includes)
            {
                query = query.Include(include);
            }

            return await query.ToListAsync();
        }

        public async Task<List<UserApplication>> GetAllAsync(Expression<Func<UserApplication, bool>> predicate, Func<IQueryable<UserApplication>, IIncludableQueryable<UserApplication, object>> includeQuery)
        {
            var query = DbSet.Where(predicate);

            query = includeQuery(query);

            return await query.ToListAsync();
        }

        public async Task<List<UserApplication>> GetAllAsync(Expression<Func<UserApplication, bool>> predicate, Func<IQueryable<UserApplication>, IOrderedQueryable<UserApplication>> sort)
        {
            var query = DbSet.Where(predicate);

            query = sort(query);

            return await query.ToListAsync();
        }

        public async Task<PaginationResponse<UserApplication>> GetAllAsync(
            Expression<Func<UserApplication, bool>> predicate,
            Func<IQueryable<UserApplication>, IOrderedQueryable<UserApplication>> sort,
            int pageIndex,
            int pageSize)
        {
            var query = DbSet.Where(predicate);
            if (sort != null)
            {
                query = sort(query);
            }

            var totalCount = await query.CountAsync();
            var items = await query
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PaginationResponse<UserApplication>
            {
                Items = items,
                TotalCount = totalCount,
                PageIndex = pageIndex,
                PageSize = pageSize
            };
        }

        public async Task<UserApplication> UpdateAsync(UserApplication entity)
        {
            DbSet.Update(entity);
            await Context.SaveChangesAsync();

            return entity;
        }

        public async Task<UserApplication> DeleteAsync(UserApplication entity)
        {
            var removedEntity = DbSet.Remove(entity).Entity;
            await Context.SaveChangesAsync();

            return removedEntity;
        }

        public async Task<bool> AnyAsync(Expression<Func<UserApplication, bool>> predicate)
        {
            return await DbSet.AnyAsync(predicate);
        }

        public async Task<List<UserApplication>> UpdateRangeAsync(List<UserApplication> entities)
        {
            DbSet.UpdateRange(entities);

            await Context.SaveChangesAsync();

            return entities;
        }



    }
}
