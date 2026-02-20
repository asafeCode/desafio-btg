using DesafioBtg.Application.Abstractions;
using DesafioBtg.Domain.Abstractions.Persistence;
using DesafioBtg.Domain.DTOs.Requests;
using DesafioBtg.Domain.DTOs.Responses;
using DesafioBtg.Domain.Enums;
using FluentValidation;

namespace DesafioBtg.Application.UseCases;

public class ProcessPixUseCase : IProcessPixUseCase
{
    private readonly IUserRepository _userRepository;
    private readonly IValidator<AccountRouteRequest> _accountValidator;
    private readonly IValidator<ProcessPixRequest> _requestValidator;

    public ProcessPixUseCase(
        IUserRepository userRepository,
        IValidator<AccountRouteRequest> accountValidator,
        IValidator<ProcessPixRequest> requestValidator)
    {
        _userRepository = userRepository;
        _accountValidator = accountValidator;
        _requestValidator = requestValidator;
    }

    public async Task<PixTransactionResponse> ExecuteAsync(
        string agencyNumber,
        string accountNumber,
        ProcessPixRequest input,
        CancellationToken cancellationToken = default)
    {
        var accountRequest = new AccountRouteRequest(agencyNumber, accountNumber);
        await _accountValidator.ValidateAndThrowAsync(accountRequest, cancellationToken);
        await _requestValidator.ValidateAndThrowAsync(input, cancellationToken);

        var result = await _userRepository.TryConsumePixLimitAsync(
            agencyNumber,
            accountNumber,
            input.Amount,
            cancellationToken);

        return result.Status switch
        {
            PixConsumeStatus.Approved => new PixTransactionResponse(true, result.RemainingLimit, "APPROVED"),
            PixConsumeStatus.AccountNotFound => new PixTransactionResponse(false, null, "ACCOUNT_NOT_FOUND"),
            PixConsumeStatus.InsufficientLimit => new PixTransactionResponse(false, result.RemainingLimit, "INSUFFICIENT_LIMIT"),
            _ => new PixTransactionResponse(false, null, "DENIED")
        };
    }
}
