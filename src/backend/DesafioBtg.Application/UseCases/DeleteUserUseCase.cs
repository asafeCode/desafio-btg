using DesafioBtg.Application.Abstractions;
using DesafioBtg.Domain.Abstractions.Persistence;
using DesafioBtg.Domain.DTOs.Requests;
using FluentValidation;

namespace DesafioBtg.Application.UseCases;

public class DeleteUserUseCase : IDeleteUserUseCase
{
    private readonly IUserRepository _userRepository;
    private readonly IValidator<AccountRouteRequest> _accountValidator;

    public DeleteUserUseCase(
        IUserRepository userRepository,
        IValidator<AccountRouteRequest> accountValidator)
    {
        _userRepository = userRepository;
        _accountValidator = accountValidator;
    }

    public async Task<bool> ExecuteAsync(string agencyNumber, string accountNumber, CancellationToken cancellationToken = default)
    {
        var accountRequest = new AccountRouteRequest(agencyNumber, accountNumber);
        await _accountValidator.ValidateAndThrowAsync(accountRequest, cancellationToken);
        
        var existing = await _userRepository.GetByAccountAsync(agencyNumber, accountNumber, cancellationToken);
        if (existing is null)
        {
            return false;
        }

        await _userRepository.DeleteAsync(
            $"AGENCY#{agencyNumber}",
            $"ACCOUNT#{accountNumber}",
            cancellationToken);

        return true;
    }
}
