namespace DesafioBtg.Domain.DTOs.Requests;

public sealed record CreateUserRequest(
    string NationalId,
    string AgencyNumber,
    string AccountNumber,
    decimal PixLimit);
