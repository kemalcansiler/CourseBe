namespace Course.Core.Interfaces;

public interface IUnitOfWork : IDisposable
{
    IGenericRepository<Entities.Course> Courses { get; }
    IGenericRepository<Entities.Category> Categories { get; }
    IUserRepository Users { get; }
    
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    Task BeginTransactionAsync(CancellationToken cancellationToken = default);
    Task CommitTransactionAsync(CancellationToken cancellationToken = default);
    Task RollbackTransactionAsync(CancellationToken cancellationToken = default);
}
