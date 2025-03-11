namespace Application.Interfaces.Repositories;

public interface IRepository<T> where T : class
{
    Task<T> AddAsync(T entity);
    Task UpdateAsync(T entity);
    Task DeleteAsync(T entity);
    IUnitOfWork UnitOfWork { get; }
    Task<HashSet<long>> GetAllIdsAsync();
    Task AddRangeAsync(List<T> entities);
}


