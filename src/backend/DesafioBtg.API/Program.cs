using Amazon.DynamoDBv2;
using DesafioBtg.API.Converters;
using DesafioBtg.API.Filters;
using DesafioBtg.Infrastructure.Migrations;
using DesafioBtg.IoC;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers().AddJsonOptions(opt =>
    opt.JsonSerializerOptions.Converters.Add(new StringConverter()));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddMvc(options => options.Filters.Add(typeof(ExceptionFilter)));
builder.Services.AddRouting(options => options.LowercaseUrls = true);

builder.Services.AddApplication(builder.Configuration);
builder.Services.AddInfrastructure(builder.Configuration);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    using var scope = app.Services.CreateScope();
    var dynamo = scope.ServiceProvider.GetRequiredService<IAmazonDynamoDB>();
    await DynamoDbMigration.EnsureUsersTableAsync(dynamo);
}

app.UseHttpsRedirection();

app.MapControllers();

app.Run();
