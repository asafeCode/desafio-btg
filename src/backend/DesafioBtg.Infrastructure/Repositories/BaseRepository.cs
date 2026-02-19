using DesafioBtg.Domain.Abstractions.Persistence;
using DesafioBtg.Domain.Entities;
using DesafioBtg.Infrastructure.DataModels;
using DesafioBtg.Infrastructure.Interfaces;

namespace DesafioBtg.Infrastructure.Repositories;

public abstract class BaseRepository<TEntity, TModel>(IDynamoDbSet<TModel> dbSet) : ICrudRepository<TEntity>
    where TEntity : EntityBase
    where TModel : BaseModel
{
    public async Task<TEntity?> GetByKeyAsync(string partitionKey, string sortKey, CancellationToken cancellationToken = default)
    {
        var model = await dbSet.GetByKeyAsync(partitionKey, sortKey, cancellationToken);
        return model is null ? null : ToEntity(model);
    }

    public async Task<IReadOnlyList<TEntity>> GetByPartitionKeyAsync(string partitionKey, CancellationToken cancellationToken = default)
    {
        var models = await dbSet.GetByPartitionKeyAsync(partitionKey, cancellationToken);
        return models.Select(ToEntity).ToList();
    }

    public Task AddAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        var model = ToModel(entity);
        return dbSet.AddOrUpdateAsync(model, cancellationToken);
    }

    public Task UpdateAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        var model = ToModel(entity);
        return dbSet.AddOrUpdateAsync(model, cancellationToken);
    }

    public Task DeleteAsync(string partitionKey, string sortKey, CancellationToken cancellationToken = default)
    {
        return dbSet.DeleteAsync(partitionKey, sortKey, cancellationToken);
    }

    protected abstract TEntity ToEntity(TModel model);
    protected abstract TModel ToModel(TEntity entity);
}
