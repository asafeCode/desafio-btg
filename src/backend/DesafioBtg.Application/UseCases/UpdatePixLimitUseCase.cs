using DesafioBtg.Application.Abstractions;
using DesafioBtg.Application.Mappers;
using DesafioBtg.Domain.Abstractions.Persistence;
using DesafioBtg.Domain.DTOs.Requests;
using DesafioBtg.Domain.DTOs.Responses;
using FluentValidation;

namespace DesafioBtg.Application.UseCases;

public class UpdatePixLimitUseCase : IUpdatePixLimitUseCase
{
    private readonly IUserRepository _userRepository;
    private readonly IValidator<AccountRouteRequest> _accountValidator;
    private readonly IValidator<UpdatePixLimitRequest> _requestValidator;

    public UpdatePixLimitUseCase(
        IUserRepository userRepository,
        IValidator<AccountRouteRequest> accountValidator,
        IValidator<UpdatePixLimitRequest> requestValidator)
    {
        _userRepository = userRepository;
        _accountValidator = accountValidator;
        _requestValidator = requestValidator;
    }

    public async Task<UserResponse?> ExecuteAsync(
        string agencyNumber,
        string accountNumber,
        UpdatePixLimitRequest input,
        CancellationToken cancellationToken = default)
    {
        var accountRequest = new AccountRouteRequest(agencyNumber, accountNumber);
        await _accountValidator.ValidateAndThrowAsync(accountRequest, cancellationToken);
        await _requestValidator.ValidateAndThrowAsync(input, cancellationToken);

        var user = await _userRepository.GetByAccountAsync(
            agencyNumber,
            accountNumber,
            cancellationToken);

        if (user is null)
            return null;
        
        user.PixLimit = input.PixLimit;
        await _userRepository.UpdateAsync(user, cancellationToken);
        return user.ToResponse();
    }
}
