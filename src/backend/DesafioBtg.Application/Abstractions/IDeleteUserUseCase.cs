namespace DesafioBtg.Application.Abstractions;

public interface IDeleteUserUseCase
{
    Task<bool> ExecuteAsync(string agencyNumber, string accountNumber, CancellationToken cancellationToken = default);
}
