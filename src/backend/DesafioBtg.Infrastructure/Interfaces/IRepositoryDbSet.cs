using DesafioBtg.Infrastructure.DataModels;

namespace DesafioBtg.Infrastructure.Interfaces;

public interface IRepositoryDbSet 
{
    IDynamoDbSet<TModel> Set<TModel>() where TModel : BaseModel;
}
