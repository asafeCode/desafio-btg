using DesafioBtg.Domain.Abstractions.Persistence;
using DesafioBtg.Domain.Entities;
using DesafioBtg.Infrastructure.DataModels;
using DesafioBtg.Infrastructure.Interfaces;

namespace DesafioBtg.Infrastructure.Repositories;

public abstract class BaseRepository<TEntity, TModel> : ICrudRepository<TEntity>
    where TEntity : EntityBase
    where TModel : BaseModel
{
    private readonly IDynamoDbSet<TModel> _dbSet;

    protected BaseRepository(IDynamoDbSet<TModel> dbSet)
    {
        _dbSet = dbSet;
    }

    public async Task<TEntity?> GetByKeyAsync(string partitionKey, string sortKey, CancellationToken cancellationToken = default)
    {
        var model = await _dbSet.GetByKeyAsync(partitionKey, sortKey, cancellationToken);
        return model is null ? null : ToEntity(model);
    }

    public async Task<IReadOnlyList<TEntity>> GetByPartitionKeyAsync(string partitionKey, CancellationToken cancellationToken = default)
    {
        var models = await _dbSet.GetByPartitionKeyAsync(partitionKey, cancellationToken);
        return models.Select(ToEntity).ToList();
    }

    public Task AddAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        var model = ToModel(entity);
        return _dbSet.AddOrUpdateAsync(model, cancellationToken);
    }

    public Task UpdateAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        var model = ToModel(entity);
        return _dbSet.AddOrUpdateAsync(model, cancellationToken);
    }

    public Task DeleteAsync(string partitionKey, string sortKey, CancellationToken cancellationToken = default)
    {
        return _dbSet.DeleteAsync(partitionKey, sortKey, cancellationToken);
    }

    protected abstract TEntity ToEntity(TModel model);
    protected abstract TModel ToModel(TEntity entity);
}
