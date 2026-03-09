using Ezz_Store.DAL.Persistance.Data;
using Ezz_Store.DAL.Persistance.Repositories;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace Ezz_Store.DAL.Persistance.UnitOfWork
{
    public class UnitOfWork(ApplicationDbContext dbContext) : IUnitOfWork
    {
        #region Fields

        private readonly ApplicationDbContext _dbContext = dbContext;
        private readonly ConcurrentDictionary<string, object> _repositories = new ConcurrentDictionary<string, object>();

        #endregion

        #region Implementation of IUnitOfWork
        public async Task<int> CompleteAsync() => await _dbContext.SaveChangesAsync();
        public ValueTask DisposeAsync() => _dbContext.DisposeAsync();

        public IGenericRepository<TEntity> GetRepository<TEntity>()
            where TEntity : class
        {
            return (IGenericRepository<TEntity>)_repositories.GetOrAdd(typeof(TEntity).Name, new GenericRepository<TEntity>(_dbContext));
        }
        #endregion
    }
}
