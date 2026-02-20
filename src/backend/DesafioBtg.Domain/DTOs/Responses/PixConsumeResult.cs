using DesafioBtg.Domain.Enums;

namespace DesafioBtg.Domain.DTOs.Responses;

public sealed record PixConsumeResult(PixConsumeStatus Status, decimal? RemainingLimit);
