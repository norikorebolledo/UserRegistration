using Core.Data.Contracts;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Core.Data.Sql.Repository
{
    public class SqlRepository<TEntity> : IDataRepository<TEntity> where TEntity : class
    {
        private DbContext _context;

        public SqlRepository(DbContext context)
        {
            _context = context;
        }

        public IQueryable<TEntity> AsQueryable()
        {
            return _context.Set<TEntity>();
        }


        public void Delete(string id)
        {
            var entity = Find(id);
            Delete(entity);
        }

        public async Task DeleteAsync(string id)
        {
            var entity = await FindAsync(id);
            Delete(entity);
        }

        public void Delete(TEntity entity)
        {
            _context.Set<TEntity>().Remove(entity);
        }

        public Task DeleteAsync(TEntity entity)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<TEntity> FilterBy(Expression<Func<TEntity, bool>> filterExpression)
        {
            return _context.Set<TEntity>().Where(filterExpression).ToList();
        }

        public IEnumerable<TProjected> FilterBy<TProjected>(Expression<Func<TEntity, bool>> filterExpression, Expression<Func<TEntity, TProjected>> projectionExpression)
        {
            return _context.Set<TEntity>().Where(filterExpression).Select(projectionExpression).ToList();
        }

        public TEntity Find(string id)
        {
            return _context.Set<TEntity>().Find(id);
        }

        public async Task<TEntity> FindAsync(string id)
        {
            return await _context.Set<TEntity>().FindAsync(id);
        }

        public TEntity Find(Expression<Func<TEntity, bool>> filterExpression)
        {
            return _context.Set<TEntity>().FirstOrDefault(filterExpression);
        }

        public Task<TEntity> FindAsync(Expression<Func<TEntity, bool>> filterExpression)
        {
            return _context.Set<TEntity>().FirstOrDefaultAsync(filterExpression);
        }

        public void Add(TEntity entity)
        {
            _context.Set<TEntity>().Add(entity);
        }

        public async Task AddAsync(TEntity entity)
        {
            await _context.Set<TEntity>().AddAsync(entity);
        }

        public void Update(TEntity entity)
        {
            _context.Set<TEntity>().Attach(entity);
            _context.Entry(entity).State = EntityState.Modified;
        }

        public Task UpdateAsync(TEntity entity)
        {
            throw new NotImplementedException();
        }
    }
}
