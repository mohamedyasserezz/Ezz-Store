using Ezz_Store.DAL.Persistance.Repositories;

namespace Ezz_Store.DAL.Persistance
{
    public interface IUnitOfWork : IAsyncDisposable
    {
        IGenericRepository<TEntity> GetRepository<TEntity>()
            where TEntity : class;
        Task<int> CompleteAsync();
    }
}
