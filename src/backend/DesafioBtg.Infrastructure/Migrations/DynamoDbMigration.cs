using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;

namespace DesafioBtg.Infrastructure.Migrations;

public static class DynamoDbMigration
{
    public static async Task EnsureUsersTableAsync(IAmazonDynamoDB dynamo, CancellationToken ct = default)
    {
        const string tableName = "users";

        await WaitForDynamoAsync(dynamo, ct);

        var tables = await dynamo.ListTablesAsync(ct);
        if (tables.TableNames.Contains(tableName))
            return;

        await dynamo.CreateTableAsync(new CreateTableRequest
        {
            TableName = tableName,
            AttributeDefinitions =
            [
                new AttributeDefinition("pk", ScalarAttributeType.S),
                new AttributeDefinition("sk", ScalarAttributeType.S),
            ],
            KeySchema =
            [
                new KeySchemaElement("pk", KeyType.HASH),
                new KeySchemaElement("sk", KeyType.RANGE),
            ],
            BillingMode = BillingMode.PAY_PER_REQUEST
        }, ct);

        var start = DateTime.UtcNow;
        while (true)
        {
            var desc = await dynamo.DescribeTableAsync(tableName, ct);
            if (desc.Table.TableStatus == TableStatus.ACTIVE) break;

            if (DateTime.UtcNow - start > TimeSpan.FromSeconds(20))
                throw new Exception($"Tabela {tableName} não ficou ACTIVE em 20s.");

            await Task.Delay(300, ct);
        }
    }

    private static async Task WaitForDynamoAsync(IAmazonDynamoDB dynamo, CancellationToken ct)
    {
        var start = DateTime.UtcNow;
        Exception? last = null;

        while (DateTime.UtcNow - start < TimeSpan.FromSeconds(20))
        {
            try
            {
                await dynamo.ListTablesAsync(ct);
                return; // ok
            }
            catch (Exception ex)
            {
                last = ex;
                await Task.Delay(500, ct);
            }
        }

        throw new Exception("DynamoDB não respondeu em 20s (startup).", last);
    }
}