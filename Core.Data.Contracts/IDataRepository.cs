using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Core.Data.Contracts
{
    public interface IDataRepository<TEntity>
    {
        IQueryable<TEntity> AsQueryable();

        IEnumerable<TEntity> FilterBy(
            Expression<Func<TEntity, bool>> filterExpression);

        IEnumerable<TProjected> FilterBy<TProjected>(
            Expression<Func<TEntity, bool>> filterExpression,
            Expression<Func<TEntity, TProjected>> projectionExpression);

        TEntity Find(string id);

        Task<TEntity> FindAsync(string id);

        TEntity Find(Expression<Func<TEntity, bool>> filterExpression);

        Task<TEntity> FindAsync(Expression<Func<TEntity, bool>> filterExpression);

        void Add(TEntity entity);

        Task AddAsync(TEntity entity);

        void Update(TEntity entity);

        Task UpdateAsync(TEntity document);

        void Delete(TEntity entity);

        Task DeleteAsync(TEntity entity);

        void Delete(string id);

        Task DeleteAsync(string id);


    }
}
