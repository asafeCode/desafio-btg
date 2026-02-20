using System.Net;
using DesafioBtg.Application.Abstractions;
using DesafioBtg.Application.Mappers;
using DesafioBtg.Domain.Abstractions.Persistence;
using DesafioBtg.Domain.DTOs.Exceptions;
using DesafioBtg.Domain.DTOs.Requests;
using DesafioBtg.Domain.DTOs.Responses;
using DesafioBtg.Domain.Entities;
using FluentValidation;

namespace DesafioBtg.Application.UseCases;

public class CreateUserUseCase : ICreateUserUseCase
{
    private readonly IUserRepository _userRepository;
    private readonly IValidator<CreateUserRequest> _validator;

    public CreateUserUseCase(
        IUserRepository userRepository,
        IValidator<CreateUserRequest> validator)
    {
        _userRepository = userRepository;
        _validator = validator;
    }

    public async Task<UserResponse> ExecuteAsync(CreateUserRequest input, CancellationToken cancellationToken = default)
    {
        await _validator.ValidateAndThrowAsync(input, cancellationToken);

        var agencyNumber = input.AgencyNumber;
        var accountNumber = input.AccountNumber;

        var existing = await _userRepository.GetByAccountAsync(agencyNumber, accountNumber, cancellationToken);
        if (existing is not null)
        {
            throw new BusinessException("Account already registered.", HttpStatusCode.Conflict);
        }

        var user = new User
        {
            NationalId = input.NationalId,
            AgencyNumber = agencyNumber,
            AccountNumber = accountNumber,
            PixLimit = input.PixLimit
        };

        await _userRepository.AddAsync(user, cancellationToken);
        return user.ToResponse();
    }
}
