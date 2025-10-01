using System.Net;

namespace Verimood.Warehouse.Application.Exceptions.Models;

public class ForbiddenException : CustomExceptionModel
{
    public ForbiddenException(string message)
        : base(message, null, HttpStatusCode.Forbidden)
    {
    }
}
