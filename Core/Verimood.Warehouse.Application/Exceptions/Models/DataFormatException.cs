using System.Net;

namespace Verimood.Warehouse.Application.Exceptions.Models;

public class DataFormatException : CustomExceptionModel
{
    public DataFormatException(string message, List<string>? errors = null) : base(message, errors, HttpStatusCode.BadRequest)
    {
    }
}
