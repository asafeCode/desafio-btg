namespace DesafioBtg.Domain.Abstractions.Persistence;

public enum PixConsumeStatus
{
    Approved = 1,
    AccountNotFound = 2,
    InsufficientLimit = 3
}

public sealed record PixConsumeResult(PixConsumeStatus Status, decimal? RemainingLimit);
