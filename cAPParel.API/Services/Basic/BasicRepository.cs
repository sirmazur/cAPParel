﻿using cAPParel.API.DbContexts;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using System.Reflection.Metadata.Ecma335;
using System.Xml.Linq;

namespace cAPParel.API.Services.Basic
{
    public class BasicRepository<TEntity> : IBasicRepository<TEntity> where TEntity : class
    {

        private readonly cAPParelContext _context;
        public BasicRepository(cAPParelContext context)
        {
            _context = context;
        }

        public async Task<TEntity> AddAsync(TEntity entity)
        {
            await _context.Set<TEntity>().AddAsync(entity);
            return entity;
        }

        public async Task<TEntity> GetByIdAsync(int id)
        {
            return await _context.Set<TEntity>().FindAsync(id);
        }

        public async Task<IEnumerable<TEntity>> GetAllAsync()
        {
            return await _context.Set<TEntity>().ToListAsync();
        }

        public IQueryable<TEntity> GetQueryableAll()
        {
            return _context.Set<TEntity>();
        }


        public async Task<(bool,TEntity?)> CheckIfIdExistsAsync(int id)
        {
            var entity = await _context.Set<TEntity>().FindAsync(id);
            return (entity != null,entity);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

        public void DeleteAsync(TEntity entity)
        {
            _context.Set<TEntity>().Remove(entity);
        }
     

        public async Task<TEntity> GetByIdWithEagerLoadingAsync(int id, params Expression<Func<TEntity, object>>[] includeProperties)
        {
            var query = _context.Set<TEntity>().AsQueryable();

            foreach (var includeProperty in includeProperties)
            {
                query = query.Include(includeProperty);
            }

            return await query.SingleOrDefaultAsync(e => EF.Property<int>(e, "Id") == id);
        }

        public IQueryable<TEntity> GetQueryableAllWithEagerLoadingAsync(params Expression<Func<TEntity, object>>[] includeProperties)
        {
            var query = _context.Set<TEntity>().AsQueryable();

            foreach (var includeProperty in includeProperties)
            {
                query = query.Include(includeProperty);
            }

            return query;
        }

    }
}
