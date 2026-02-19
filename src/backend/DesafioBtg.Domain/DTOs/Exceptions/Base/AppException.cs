using System.Net;

namespace DesafioBtg.Domain.DTOs.Exceptions.Base;

public abstract class AppException : SystemException
{
    protected AppException(string messages) : base(messages)
    {
    }

    public abstract HttpStatusCode GetStatusCode();
    public abstract IList<string> GetErrorMessages();
}
