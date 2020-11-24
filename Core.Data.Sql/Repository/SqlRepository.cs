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
            await DeleteAsync(entity);
        }

        public void Delete(TEntity entity)
        {
            _context.Set<TEntity>().Remove(entity);
            _context.SaveChanges();
        }

        public async Task DeleteAsync(TEntity entity)
        {
            _context.Set<TEntity>().Remove(entity);
            await _context.SaveChangesAsync();
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
            _context.SaveChanges();
        }

        public async Task AddAsync(TEntity entity)
        {
            await _context.Set<TEntity>().AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public void Update(TEntity entity)
        {
            _context.Set<TEntity>().Attach(entity);
            _context.Entry(entity).State = EntityState.Modified;
        }

        public async Task UpdateAsync(TEntity entity)
        {
            Update(entity);
            await _context.SaveChangesAsync();
        }
    }
}
