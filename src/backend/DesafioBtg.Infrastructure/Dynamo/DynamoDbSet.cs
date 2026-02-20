using Amazon.DynamoDBv2.DataModel;
using DesafioBtg.Infrastructure.DataModels;
using DesafioBtg.Infrastructure.Interfaces;

namespace DesafioBtg.Infrastructure.Dynamo;

public class DynamoDbSet<TModel> : IDynamoDbSet<TModel> where TModel : BaseModel
{
    private readonly IDynamoDBContext _dynamoDbContext;

    public DynamoDbSet(IDynamoDBContext dynamoDbContext)
    {
        _dynamoDbContext = dynamoDbContext;
    }

    public async Task<TModel?> GetByKeyAsync(string partitionKey, string sortKey, CancellationToken cancellationToken = default)
        => await _dynamoDbContext.LoadAsync<TModel>(partitionKey, sortKey, cancellationToken);

    public async Task<IReadOnlyList<TModel>> GetByPartitionKeyAsync(string partitionKey, CancellationToken cancellationToken = default)
    {
        var query = _dynamoDbContext.QueryAsync<TModel>(partitionKey);
        var items = await query.GetRemainingAsync(cancellationToken);
        return items;
    }

    public Task AddOrUpdateAsync(TModel model, CancellationToken cancellationToken = default)
    {
        return _dynamoDbContext.SaveAsync(model, cancellationToken);
    }

    public Task DeleteAsync(string partitionKey, string sortKey, CancellationToken cancellationToken = default)
    {
        return _dynamoDbContext.DeleteAsync<TModel>(partitionKey, sortKey, cancellationToken);
    }
}
