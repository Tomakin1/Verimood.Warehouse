namespace Verimood.Warehouse.Application.Exceptions.Models;

class ConcurentRequestException : CustomExceptionModel
{

    public ConcurentRequestException(string message) : base(message, null, System.Net.HttpStatusCode.AlreadyReported)
    {

    }
}
