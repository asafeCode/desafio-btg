using Amazon.DynamoDBv2.DataModel;

namespace DesafioBtg.Infrastructure.DataModels;

public class BaseModel
{
    [DynamoDBHashKey("pk")]
    public string Pk { get; set; } = default!;

    [DynamoDBRangeKey("sk")]
    public string Sk { get; set; } = default!;

    [DynamoDBProperty("created_at")]
    public DateTime CreatedAt { get; set; }

    [DynamoDBProperty("active")]
    public bool Active { get; set; }
}
