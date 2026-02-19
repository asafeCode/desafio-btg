using DesafioBtg.Domain.DTOs.Requests;
using DesafioBtg.Domain.DTOs.Responses;

namespace DesafioBtg.Application.Abstractions;

public interface IUpdatePixLimitUseCase
{
    Task<UserResponse?> ExecuteAsync(
        string agencyNumber,
        string accountNumber,
        UpdatePixLimitRequest input,
        CancellationToken cancellationToken = default);
}
