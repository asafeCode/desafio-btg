using DesafioBtg.Domain.DTOs.Responses;
using DesafioBtg.Domain.Entities;

namespace DesafioBtg.Domain.Abstractions.Persistence;

public interface IUserRepository : ICrudRepository<User>
{
    Task<User?> GetByAccountAsync(string agencyNumber, string accountNumber, CancellationToken cancellationToken = default);
    Task<PixConsumeResult> TryConsumePixLimitAsync(
        string agencyNumber,
        string accountNumber,
        decimal amount,
        CancellationToken cancellationToken = default);
}
