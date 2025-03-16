namespace Application.Interfaces.Repositories;

public interface IRepository<T> where T : class
{
    IUnitOfWork UnitOfWork { get; }
    Task<T> AddAsync(T entity);
    Task UpdateAsync(T entity);
    Task DeleteAsync(T entity);
    Task<HashSet<long>> GetAllIdsAsync();
    Task AddRangeAsync(List<T> entities);
}
