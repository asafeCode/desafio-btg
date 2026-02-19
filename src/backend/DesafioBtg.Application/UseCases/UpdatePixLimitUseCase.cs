using DesafioBtg.Application.Abstractions;
using DesafioBtg.Application.UseCases.Shared;
using DesafioBtg.Domain.Abstractions.Persistence;
using DesafioBtg.Domain.DTOs.Requests;
using DesafioBtg.Domain.DTOs.Responses;
using FluentValidation;

namespace DesafioBtg.Application.UseCases;

public class UpdatePixLimitUseCase(
    IUserRepository userRepository,
    IValidator<AccountRouteRequest> accountValidator,
    IValidator<UpdatePixLimitRequest> requestValidator) : IUpdatePixLimitUseCase
{
    public async Task<UserResponse?> ExecuteAsync(
        string agencyNumber,
        string accountNumber,
        UpdatePixLimitRequest input,
        CancellationToken cancellationToken = default)
    {
        var accountRequest = new AccountRouteRequest(agencyNumber, accountNumber);
        await accountValidator.ValidateAndThrowAsync(accountRequest, cancellationToken);
        await requestValidator.ValidateAndThrowAsync(input, cancellationToken);

        var user = await userRepository.GetByAccountAsync(
            UserUseCaseMapper.Normalize(agencyNumber),
            UserUseCaseMapper.Normalize(accountNumber),
            cancellationToken);

        if (user is null)
        {
            return null;
        }

        user.PixLimit = input.PixLimit;
        await userRepository.UpdateAsync(user, cancellationToken);
        return UserUseCaseMapper.ToResponse(user);
    }
}
