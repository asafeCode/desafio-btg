using System.Net;
using DesafioBtg.Application.Abstractions;
using DesafioBtg.Application.UseCases.Shared;
using DesafioBtg.Domain.Abstractions.Persistence;
using DesafioBtg.Domain.DTOs.Exceptions;
using DesafioBtg.Domain.DTOs.Requests;
using DesafioBtg.Domain.DTOs.Responses;
using DesafioBtg.Domain.Entities;
using FluentValidation;

namespace DesafioBtg.Application.UseCases;

public class CreateUserUseCase(
    IUserRepository userRepository,
    IValidator<CreateUserRequest> validator) : ICreateUserUseCase
{
    public async Task<UserResponse> ExecuteAsync(CreateUserRequest input, CancellationToken cancellationToken = default)
    {
        await validator.ValidateAndThrowAsync(input, cancellationToken);

        var agencyNumber = UserUseCaseMapper.Normalize(input.AgencyNumber);
        var accountNumber = UserUseCaseMapper.Normalize(input.AccountNumber);

        var existing = await userRepository.GetByAccountAsync(agencyNumber, accountNumber, cancellationToken);
        if (existing is not null)
        {
            throw new BusinessException("Account already registered.", HttpStatusCode.Conflict);
        }

        var user = new User
        {
            NationalId = UserUseCaseMapper.Normalize(input.NationalId),
            AgencyNumber = agencyNumber,
            AccountNumber = accountNumber,
            PixLimit = input.PixLimit
        };

        await userRepository.AddAsync(user, cancellationToken);
        return UserUseCaseMapper.ToResponse(user);
    }
}
