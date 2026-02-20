using DesafioBtg.Application.Abstractions;
using DesafioBtg.Application.Mappers;
using DesafioBtg.Domain.Abstractions.Persistence;
using DesafioBtg.Domain.DTOs.Requests;
using DesafioBtg.Domain.DTOs.Responses;
using FluentValidation;

namespace DesafioBtg.Application.UseCases;

public class GetUserByAccountUseCase : IGetUserByAccountUseCase
{
    private readonly IUserRepository _userRepository;
    private readonly IValidator<AccountRouteRequest> _accountValidator;

    public GetUserByAccountUseCase(
        IUserRepository userRepository,
        IValidator<AccountRouteRequest> accountValidator)
    {
        _userRepository = userRepository;
        _accountValidator = accountValidator;
    }

    public async Task<UserResponse?> ExecuteAsync(string agencyNumber, string accountNumber, CancellationToken cancellationToken = default)
    {
        var accountRequest = new AccountRouteRequest(agencyNumber, accountNumber);
        await _accountValidator.ValidateAndThrowAsync(accountRequest, cancellationToken);

        var user = await _userRepository.GetByAccountAsync(
            agencyNumber,
            accountNumber,
            cancellationToken);

        return user?.ToResponse();
    }
}
