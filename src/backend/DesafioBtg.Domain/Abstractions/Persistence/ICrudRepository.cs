using DesafioBtg.Domain.Entities;

namespace DesafioBtg.Domain.Abstractions.Persistence;

public interface ICrudRepository<TEntity> where TEntity : EntityBase
{
    Task<TEntity?> GetByKeyAsync(string partitionKey, string sortKey, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<TEntity>> GetByPartitionKeyAsync(string partitionKey, CancellationToken cancellationToken = default);
    Task AddAsync(TEntity entity, CancellationToken cancellationToken = default);
    Task UpdateAsync(TEntity entity, CancellationToken cancellationToken = default);
    Task DeleteAsync(string partitionKey, string sortKey, CancellationToken cancellationToken = default);
}
