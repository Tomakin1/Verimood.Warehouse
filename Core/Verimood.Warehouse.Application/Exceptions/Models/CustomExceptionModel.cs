using System.Net;

namespace Verimood.Warehouse.Application.Exceptions.Models;

public class CustomExceptionModel : Exception
{
    public List<string>? ErrorMessages { get; set; }
    public HttpStatusCode StatusCode { get; set; }
    public string ThrowMessage { get; set; }


    public CustomExceptionModel(string message, List<string>? errors = default
        , HttpStatusCode statusCode = HttpStatusCode.InternalServerError)
    {
        ThrowMessage = message;
        ErrorMessages = errors;
        StatusCode = statusCode;
    }
}
