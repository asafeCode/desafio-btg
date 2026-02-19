namespace DesafioBtg.Domain.DTOs.Responses;

public sealed record PixTransactionResponse(
    bool Approved,
    decimal? RemainingLimit,
    string Reason);
