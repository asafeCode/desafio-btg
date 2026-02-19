using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using DesafioBtg.Domain.Abstractions.Persistence;
using DesafioBtg.Domain.Entities;
using DesafioBtg.Infrastructure.DataModels;
using DesafioBtg.Infrastructure.Interfaces;

namespace DesafioBtg.Infrastructure.Repositories;

public class UserRepository(IDynamoDbSet<UserModel> dbSet, IAmazonDynamoDB dynamoDbClient)
    : BaseRepository<User, UserModel>(dbSet), IUserRepository
{
    private const string TableName = "users";

    public Task<User?> GetByAccountAsync(string agencyNumber, string accountNumber, CancellationToken cancellationToken = default)
    {
        var partitionKey = BuildPartitionKey(agencyNumber);
        var sortKey = BuildSortKey(accountNumber);
        
        return GetByKeyAsync(partitionKey, sortKey, cancellationToken);
    }

    public async Task<PixConsumeResult> TryConsumePixLimitAsync(
        string agencyNumber,
        string accountNumber,
        decimal amount,
        CancellationToken cancellationToken = default)
    {
        var partitionKey = BuildPartitionKey(agencyNumber);
        var sortKey = BuildSortKey(accountNumber);

        var request = new UpdateItemRequest
        {
            TableName = TableName,
            Key = new Dictionary<string, AttributeValue>
            {
                ["pk"] = new() { S = partitionKey },
                ["sk"] = new() { S = sortKey }
            },
            UpdateExpression = "SET pix_limit = pix_limit - :amount",
            ConditionExpression = "attribute_exists(pk) AND attribute_exists(sk) AND pix_limit >= :amount",
            ExpressionAttributeValues = new Dictionary<string, AttributeValue>
            {
                [":amount"] = new() { N = amount.ToString(System.Globalization.CultureInfo.InvariantCulture) }
            },
            ReturnValues = ReturnValue.ALL_NEW
        };

        try
        {
            var response = await dynamoDbClient.UpdateItemAsync(request, cancellationToken);
            var remainingLimit = decimal.Parse(
                response.Attributes["pix_limit"].N,
                System.Globalization.CultureInfo.InvariantCulture);

            return new PixConsumeResult(PixConsumeStatus.Approved, remainingLimit);
        }
        catch (ConditionalCheckFailedException)
        {
            var account = await GetByAccountAsync(agencyNumber, accountNumber, cancellationToken);
            if (account is null)
            {
                return new PixConsumeResult(PixConsumeStatus.AccountNotFound, null);
            }

            return new PixConsumeResult(PixConsumeStatus.InsufficientLimit, account.PixLimit);
        }
    }
    
    protected override User ToEntity(UserModel model)
    {
        return new User
        {
            CreatedAt = model.CreatedAt,
            Active = model.Active,
            NationalId = model.NationalId,
            AgencyNumber = model.AgencyNumber,
            AccountNumber = model.AccountNumber,
            PixLimit = model.PixLimit
        };
    }

    protected override UserModel ToModel(User entity)
    {
        return new UserModel
        {
            Pk = BuildPartitionKey(entity.AgencyNumber),
            Sk = BuildSortKey(entity.AccountNumber),
            CreatedAt = entity.CreatedAt,
            Active = entity.Active,
            NationalId = entity.NationalId,
            AgencyNumber = entity.AgencyNumber,
            AccountNumber = entity.AccountNumber,
            PixLimit = entity.PixLimit
        };
    }

    public static string BuildPartitionKey(string agencyNumber) => $"AGENCY#{agencyNumber}";
    public static string BuildSortKey(string accountNumber) => $"ACCOUNT#{accountNumber}";
}
