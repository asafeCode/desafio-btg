using DesafioBtg.Domain.DTOs.Responses;

namespace DesafioBtg.Application.Abstractions;

public interface IGetUserByAccountUseCase
{
    Task<UserResponse?> ExecuteAsync(string agencyNumber, string accountNumber, CancellationToken cancellationToken = default);
}
