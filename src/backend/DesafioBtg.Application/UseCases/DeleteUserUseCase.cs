using DesafioBtg.Application.Abstractions;
using DesafioBtg.Application.UseCases.Shared;
using DesafioBtg.Domain.Abstractions.Persistence;
using DesafioBtg.Domain.DTOs.Requests;
using FluentValidation;

namespace DesafioBtg.Application.UseCases;

public class DeleteUserUseCase(
    IUserRepository userRepository,
    IValidator<AccountRouteRequest> accountValidator) : IDeleteUserUseCase
{
    public async Task<bool> ExecuteAsync(string agencyNumber, string accountNumber, CancellationToken cancellationToken = default)
    {
        var accountRequest = new AccountRouteRequest(agencyNumber, accountNumber);
        await accountValidator.ValidateAndThrowAsync(accountRequest, cancellationToken);

        var normalizedAgency = UserUseCaseMapper.Normalize(agencyNumber);
        var normalizedAccount = UserUseCaseMapper.Normalize(accountNumber);

        var existing = await userRepository.GetByAccountAsync(normalizedAgency, normalizedAccount, cancellationToken);
        if (existing is null)
        {
            return false;
        }

        await userRepository.DeleteAsync(
            $"AGENCY#{normalizedAgency}",
            $"ACCOUNT#{normalizedAccount}",
            cancellationToken);

        return true;
    }
}
