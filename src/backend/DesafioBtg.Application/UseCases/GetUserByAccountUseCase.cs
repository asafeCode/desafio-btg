using DesafioBtg.Application.Abstractions;
using DesafioBtg.Application.UseCases.Shared;
using DesafioBtg.Domain.Abstractions.Persistence;
using DesafioBtg.Domain.DTOs.Requests;
using DesafioBtg.Domain.DTOs.Responses;
using FluentValidation;

namespace DesafioBtg.Application.UseCases;

public class GetUserByAccountUseCase(
    IUserRepository userRepository,
    IValidator<AccountRouteRequest> accountValidator) : IGetUserByAccountUseCase
{
    public async Task<UserResponse?> ExecuteAsync(string agencyNumber, string accountNumber, CancellationToken cancellationToken = default)
    {
        var accountRequest = new AccountRouteRequest(agencyNumber, accountNumber);
        await accountValidator.ValidateAndThrowAsync(accountRequest, cancellationToken);

        var user = await userRepository.GetByAccountAsync(
            UserUseCaseMapper.Normalize(agencyNumber),
            UserUseCaseMapper.Normalize(accountNumber),
            cancellationToken);

        return user is null ? null : UserUseCaseMapper.ToResponse(user);
    }
}
