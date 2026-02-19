namespace DesafioBtg.Domain.DTOs.Requests;

public sealed record AccountRouteRequest(
    string AgencyNumber,
    string AccountNumber);
