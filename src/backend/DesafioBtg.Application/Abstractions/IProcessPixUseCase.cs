using DesafioBtg.Domain.DTOs.Requests;
using DesafioBtg.Domain.DTOs.Responses;

namespace DesafioBtg.Application.Abstractions;

public interface IProcessPixUseCase
{
    Task<PixTransactionResponse> ExecuteAsync(
        string agencyNumber,
        string accountNumber,
        ProcessPixRequest input,
        CancellationToken cancellationToken = default);
}
