namespace DesafioBtg.Domain.DTOs.Responses;

public sealed record UserResponse(
    string NationalId,
    string AgencyNumber,
    string AccountNumber,
    decimal PixLimit,
    DateTime CreatedAt,
    bool Active);
