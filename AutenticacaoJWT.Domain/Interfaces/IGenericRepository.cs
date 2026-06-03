using AutenticacaoJWT.Domain.Pagination;

namespace AutenticacaoJWT.Domain.Interfaces
{
    public interface IGenericRepository<TEntity> where TEntity : class
    {
        Task<TEntity?> GetById(int id);
        Task<IEnumerable<TEntity>> GetAll();
        Task<PagedList<TEntity>> GetAllPagination(int pageNumber, int pageSize);
        Task<TEntity> Create(TEntity entity);
        Task<TEntity> Update(TEntity entity);
        Task<TEntity> Remove(TEntity entity);
        void Save();
    }
}
