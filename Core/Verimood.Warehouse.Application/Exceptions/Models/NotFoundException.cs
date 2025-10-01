using System.Net;

namespace Verimood.Warehouse.Application.Exceptions.Models;

public class NotFoundException : CustomExceptionModel
{
    public NotFoundException(string message) : base(message, null, HttpStatusCode.NotFound)
    {
    }

    public static void ThrowIfNull(object? value, string message)
    {
        if (value == null) throw new NotFoundException(message);
    }

    public static void ThrowIfNullEntity(object? value)
    {
        if (value == null) throw new NotFoundException("Kayıt bulunamadı");
    }
}
