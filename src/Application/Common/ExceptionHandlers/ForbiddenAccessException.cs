using System.Net;

namespace CleanArchitecture.Blazor.Application.Common.ExceptionHandlers;

public class ForbiddenException : ServerException
{
    public ForbiddenException(string message) : base(message, HttpStatusCode.Forbidden) { }
}