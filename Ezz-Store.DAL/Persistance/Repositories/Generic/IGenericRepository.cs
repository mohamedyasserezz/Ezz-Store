using System;
using System.Collections.Generic;
using System.Text;

namespace Ezz_Store.DAL.Persistance.Repositories
{
    public interface IGenericRepository<T> where T : class
    {
        Task<IEnumerable<T>> GetAllAsync();
        public IQueryable<T> GetIQueryable();
        public IEnumerable<T> GetIEnumerable();
        Task<T?> GetAsync(int id);
        void Add(T entity);
        void Update(T entity);
        void Delete(T entity);
    }
}
