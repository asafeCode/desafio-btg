using DesafioBtg.Domain.DTOs.Responses;
using DesafioBtg.Domain.Entities;

namespace DesafioBtg.Application.Mappers;

public static class DomainToResponse
{
    internal static UserResponse ToResponse(this User user) =>
        new(user.NationalId, user.AgencyNumber, user.AccountNumber, user.PixLimit, user.CreatedAt, user.Active);
}