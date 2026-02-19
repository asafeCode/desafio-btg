using DesafioBtg.Infrastructure.DataModels;

namespace DesafioBtg.Infrastructure.Interfaces;

public interface IDynamoDbSet<TModel> where TModel : BaseModel
{
    Task<TModel?> GetByKeyAsync(string partitionKey, string sortKey, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<TModel>> GetByPartitionKeyAsync(string partitionKey, CancellationToken cancellationToken = default);
    Task AddOrUpdateAsync(TModel model, CancellationToken cancellationToken = default);
    Task DeleteAsync(string partitionKey, string sortKey, CancellationToken cancellationToken = default);
}
