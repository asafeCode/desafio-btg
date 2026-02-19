using Aspire.Hosting.AWS.DynamoDB;

var builder = DistributedApplication.CreateBuilder(args);

builder.AddAWSSDKConfig();

var localDynamo = builder.AddAWSDynamoDBLocal("dynamodb");

builder.AddContainer("dynamodb-admin", "aaronshaf/dynamodb-admin")
    .WithReference(localDynamo)
    .WithEnvironment("DYNAMO_ENDPOINT", localDynamo.GetEndpoint("http"))
    .WithEnvironment("AWS_ACCESS_KEY_ID", "test")
    .WithEnvironment("AWS_SECRET_ACCESS_KEY", "test")
    .WithEnvironment("AWS_DEFAULT_REGION", "us-east-1")
    .WithEnvironment("AWS_REGION", "us-east-1")
    .WithHttpEndpoint(port: 8001, targetPort: 8001);

builder.AddProject<Projects.DesafioBtg_API>("desafioBtg-Api")
    .WithReference(localDynamo);

builder.Build().Run();