using Amazon.DynamoDBv2.DataModel;

namespace DesafioBtg.Infrastructure.DataModels;

[DynamoDBTable("users")]
public class UserModel : BaseModel
{
    [DynamoDBProperty("cpf")]
    public string NationalId { get; set; } = default!;

    [DynamoDBProperty("agency_number")]
    public string AgencyNumber { get; set; } = default!;

    [DynamoDBProperty("account_number")]
    public string AccountNumber { get; set; } = default!;

    [DynamoDBProperty("pix_limit")]
    public decimal PixLimit { get; set; }
}
