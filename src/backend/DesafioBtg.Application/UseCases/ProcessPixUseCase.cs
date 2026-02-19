using DesafioBtg.Application.Abstractions;
using DesafioBtg.Application.UseCases.Shared;
using DesafioBtg.Domain.Abstractions.Persistence;
using DesafioBtg.Domain.DTOs.Requests;
using DesafioBtg.Domain.DTOs.Responses;
using FluentValidation;

namespace DesafioBtg.Application.UseCases;

public class ProcessPixUseCase(
    IUserRepository userRepository,
    IValidator<AccountRouteRequest> accountValidator,
    IValidator<ProcessPixRequest> requestValidator) : IProcessPixUseCase
{
    public async Task<PixTransactionResponse> ExecuteAsync(
        string agencyNumber,
        string accountNumber,
        ProcessPixRequest input,
        CancellationToken cancellationToken = default)
    {
        var accountRequest = new AccountRouteRequest(agencyNumber, accountNumber);
        await accountValidator.ValidateAndThrowAsync(accountRequest, cancellationToken);
        await requestValidator.ValidateAndThrowAsync(input, cancellationToken);

        var result = await userRepository.TryConsumePixLimitAsync(
            UserUseCaseMapper.Normalize(agencyNumber),
            UserUseCaseMapper.Normalize(accountNumber),
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
