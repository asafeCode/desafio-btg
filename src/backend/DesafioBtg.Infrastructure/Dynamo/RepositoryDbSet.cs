using DesafioBtg.Infrastructure.DataModels;
using DesafioBtg.Infrastructure.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace DesafioBtg.Infrastructure.Dynamo;

public class RepositoryDbSet(IServiceProvider serviceProvider) : IRepositoryDbSet
{
    public IDynamoDbSet<TModel> Set<TModel>() where TModel : BaseModel
    {
        return serviceProvider.GetRequiredService<IDynamoDbSet<TModel>>();
    }
}
