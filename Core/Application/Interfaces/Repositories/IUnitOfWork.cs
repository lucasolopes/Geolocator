namespace Application.Interfaces.Repositories;

public interface IUnitOfWork
{
    Task<int> SaveChangesAsync(System.Threading.CancellationToken cancellationToken = default);
}
