using System.Net;

namespace Verimood.Warehouse.Application.Exceptions.Models;

public class InternalServerException : CustomExceptionModel
{
    public InternalServerException(string message, List<string>? errors = default)
        : base(message, errors, HttpStatusCode.InternalServerError)
    {
    }
}
