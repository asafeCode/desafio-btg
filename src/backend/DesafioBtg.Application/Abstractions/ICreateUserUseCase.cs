using DesafioBtg.Domain.DTOs.Requests;
using DesafioBtg.Domain.DTOs.Responses;

namespace DesafioBtg.Application.Abstractions;

public interface ICreateUserUseCase
{
    Task<UserResponse> ExecuteAsync(CreateUserRequest input, CancellationToken cancellationToken = default);
}
