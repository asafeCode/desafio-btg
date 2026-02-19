using DesafioBtg.Domain.DTOs.Responses;
using DesafioBtg.Domain.Entities;

namespace DesafioBtg.Application.UseCases.Shared;

internal static class UserUseCaseMapper
{
    internal static UserResponse ToResponse(User user) =>
        new(user.NationalId, user.AgencyNumber, user.AccountNumber, user.PixLimit, user.CreatedAt, user.Active);

    internal static string Normalize(string value) => value.Trim();
}
