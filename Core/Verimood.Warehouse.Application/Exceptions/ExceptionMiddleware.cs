using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Serilog;
using Serilog.Context;
using System.Net;
using Verimood.Warehouse.Application.Exceptions.Models;

namespace Verimood.Warehouse.Application.Exceptions;

public class ExceptionMiddleware : IMiddleware
{
    public async Task InvokeAsync(HttpContext htppContext, RequestDelegate next)
    {
        try
        {
            await next(htppContext);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(htppContext, ex);
        }
    }

    private static Task HandleExceptionAsync(HttpContext context, Exception ex)
    {
        string errorId = Guid.NewGuid().ToString();
        var errorResult = new ErrorResultDto
        {
            Source = ex.TargetSite?.DeclaringType?.FullName,
            Exception = ex.Message.Trim(),
            ErrorId = errorId,
            SupportMessage = string.Empty,

        };

        LogContext.PushProperty("ErrorId", errorId);
        LogContext.PushProperty("StackTrace", ex.StackTrace);

        errorResult.Messages.Add(ex.Message);

        if (ex is not CustomExceptionModel && ex.InnerException != null)
        {
            while (ex.InnerException != null)
            {
                ex = ex.InnerException;
            }
        }

        switch (ex)
        {
            case CustomExceptionModel e:
                errorResult.StatusCode = (int)e.StatusCode;
                if (e.ErrorMessages is not null)
                {
                    errorResult.Messages = e.ErrorMessages;
                }
                if (e.ThrowMessage is not null)
                {
                    errorResult.ThrowMessage = e.ThrowMessage;
                }
                break;
            case KeyNotFoundException:
                errorResult.StatusCode = (int)HttpStatusCode.NotFound;
                break;
            default:
                errorResult.StatusCode = (int)HttpStatusCode.InternalServerError;
                break;
        }

        Log.Error($"{errorResult.Exception} Request failed with Status Code {context.Response.StatusCode} and Error Id {errorId}.");
        Log.Error(ex, "Request failed with error");

        var response = context.Response;
        if (!response.HasStarted)
        {
            response.ContentType = "application/json";
            response.StatusCode = errorResult.StatusCode;
            var jsonResponse = JsonConvert.SerializeObject(errorResult);
            return context.Response.WriteAsync(jsonResponse);
        }
        else
        {
            throw new ConflictException("Response has already started, the response cannot be modified.");
        }
    }
}
