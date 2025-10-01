using System.Net;

namespace Verimood.Warehouse.Application.Exceptions.Models;

public class ConflictException : CustomExceptionModel
{
    public ConflictException(string message)
        : base(message, null, HttpStatusCode.Conflict)
    {
    }
}
