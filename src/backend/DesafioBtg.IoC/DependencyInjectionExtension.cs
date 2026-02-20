using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.Runtime;
using DesafioBtg.Application.Abstractions;
using DesafioBtg.Application.UseCases;
using DesafioBtg.Application.Validators;
using DesafioBtg.Domain.DTOs.Requests;
using DesafioBtg.Domain.Abstractions.Persistence;
using DesafioBtg.Infrastructure.Dynamo;
using DesafioBtg.Infrastructure.Interfaces;
using DesafioBtg.Infrastructure.Repositories;
using FluentValidation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DesafioBtg.IoC;

public static class DependencyInjectionExtension
{
    public static void AddApplication(this IServiceCollection services, IConfiguration configuration)
    {
        AddUseCases(services);
    }

    public static void AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        AddDynamoDb(services, configuration);
    }

    private static void AddDynamoDb(IServiceCollection services, IConfiguration configuration)
    {
        var dynamoEndpoint = configuration["AWS_ENDPOINT_URL_DYNAMODB"];
        if (string.IsNullOrWhiteSpace(dynamoEndpoint))
            throw new InvalidOperationException("Endpoint não encontrado.");

        services.AddSingleton<IAmazonDynamoDB>(_ =>
        {
            var cfg = new AmazonDynamoDBConfig
            {
                ServiceURL = dynamoEndpoint,
                UseHttp = dynamoEndpoint.StartsWith("http://", StringComparison.OrdinalIgnoreCase)
            };

            return new AmazonDynamoDBClient(
                new BasicAWSCredentials("test", "test"),
                cfg);
        });

        services.AddSingleton<IDynamoDBContext>(sp =>
            new DynamoDBContextBuilder()
                .WithDynamoDBClient(() => sp.GetRequiredService<IAmazonDynamoDB>())
                .Build());

        services.AddScoped(typeof(IDynamoDbSet<>), typeof(DynamoDbSet<>));
        services.AddScoped<IUserRepository, UserRepository>();
    }

    private static void AddUseCases(IServiceCollection services)
    {
        services.AddScoped<ICreateUserUseCase, CreateUserUseCase>();
        services.AddScoped<IGetUserByAccountUseCase, GetUserByAccountUseCase>();
        services.AddScoped<IUpdatePixLimitUseCase, UpdatePixLimitUseCase>();
        services.AddScoped<IDeleteUserUseCase, DeleteUserUseCase>();
        services.AddScoped<IProcessPixUseCase, ProcessPixUseCase>();

        services.AddScoped<IValidator<CreateUserRequest>, CreateUserRequestValidator>();
        services.AddScoped<IValidator<AccountRouteRequest>, AccountRouteRequestValidator>();
        services.AddScoped<IValidator<UpdatePixLimitRequest>, UpdatePixLimitRequestValidator>();
        services.AddScoped<IValidator<ProcessPixRequest>, ProcessPixRequestValidator>();
    }
}
