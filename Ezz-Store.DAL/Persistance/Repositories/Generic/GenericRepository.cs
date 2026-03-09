using Ezz_Store.DAL.Persistance.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ezz_Store.DAL.Persistance.Repositories
{
    public class GenericRepository<T>(ApplicationDbContext dbContext) : IGenericRepository<T> where T : class
    {
        private readonly ApplicationDbContext _dbContext = dbContext;

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _dbContext.Set<T>().ToListAsync();
        }
        public IQueryable<T> GetIQueryable()
        {
            return _dbContext.Set<T>();
        }
        public IEnumerable<T> GetIEnumerable()
        {
            throw new NotImplementedException();
        }
        public async Task<T?> GetAsync(int id)
        {
            return await _dbContext.Set<T>().FindAsync(id);

        }
        public void Add(T entity) => _dbContext.Set<T>().Add(entity);

        public void Update(T entity) => _dbContext.Update(entity);

        public void Delete(T entity)
        {   _dbContext.Set<T>().Remove(entity); }

    }
}
