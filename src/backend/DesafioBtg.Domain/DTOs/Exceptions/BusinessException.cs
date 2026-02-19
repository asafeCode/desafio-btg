using System.Net;
using DesafioBtg.Domain.DTOs.Exceptions.Base;

namespace DesafioBtg.Domain.DTOs.Exceptions;

public sealed class BusinessException : AppException
{
    private readonly IList<string> _errors;
    private readonly HttpStatusCode _statusCode;

    public BusinessException(string messages, HttpStatusCode statusCode = HttpStatusCode.BadRequest) : base(messages)
    {
        _statusCode = statusCode;
        _errors = [Message];
    }

    public override HttpStatusCode GetStatusCode() => _statusCode;

    public override IList<string> GetErrorMessages() => _errors;
}
