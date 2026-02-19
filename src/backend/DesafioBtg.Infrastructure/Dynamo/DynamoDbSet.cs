using Amazon.DynamoDBv2.DataModel;
using DesafioBtg.Infrastructure.DataModels;
using DesafioBtg.Infrastructure.Interfaces;

namespace DesafioBtg.Infrastructure.Dynamo;

public class DynamoDbSet<TModel>(IDynamoDBContext dynamoDbContext) : IDynamoDbSet<TModel> where TModel : BaseModel
{
    public async Task<TModel?> GetByKeyAsync(string partitionKey, string sortKey, CancellationToken cancellationToken = default)
        => await dynamoDbContext.LoadAsync<TModel>(partitionKey, sortKey, cancellationToken);
    

    public async Task<IReadOnlyList<TModel>> GetByPartitionKeyAsync(string partitionKey, CancellationToken cancellationToken = default)
    {
        var query = dynamoDbContext.QueryAsync<TModel>(partitionKey);
        var items = await query.GetRemainingAsync(cancellationToken);
        return items;
    }

    public Task AddOrUpdateAsync(TModel model, CancellationToken cancellationToken = default)
    {
        return dynamoDbContext.SaveAsync(model, cancellationToken);
    }

    public Task DeleteAsync(string partitionKey, string sortKey, CancellationToken cancellationToken = default)
    {
        return dynamoDbContext.DeleteAsync<TModel>(partitionKey, sortKey, cancellationToken);
    }
}
