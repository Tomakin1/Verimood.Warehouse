using System.Net;

namespace Verimood.Warehouse.Application.Exceptions.Models;


public class UnauthorizedException : CustomExceptionModel
{
    public UnauthorizedException(string message)
       : base(message, null, HttpStatusCode.Unauthorized)
    {
    }
}
