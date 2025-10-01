using System.Text.Json.Serialization;

namespace Verimood.Warehouse.Application.Helpers;

public class BaseResponse<T>
{
    public bool Success { get; set; }
    public string? Message { get; set; }
    public T? Data { get; set; }
    public List<string>? Errors { get; set; }
    [JsonIgnore]
    public int StatusCode { get; set; } = 200;
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;

    public BaseResponse(bool success, string? message = null, T? data = default, List<string>? errors = null)
    {
        Success = success;
        Message = message;
        Data = data;
        Errors = errors;
    }

    public static BaseResponse<T> SuccessResponse(T data, string? message = null)
        => new BaseResponse<T>(true, message, data);
    public static BaseResponse<T> SuccessResponse(T data, int statusCode, string? message = null)
    {
        var response = new BaseResponse<T>(true, message, data, null);
        response.StatusCode = statusCode;
        return response;
    }

    public static BaseResponse<T> SuccessResponse(string message)
        => new BaseResponse<T>(true, message);

    public static BaseResponse<T> ErrorResponse(string message)
        => new BaseResponse<T>(false, message);

    public static BaseResponse<T> ErrorResponse(List<string> errors, string? message = null)
        => new BaseResponse<T>(false, message, default, errors);

    public static BaseResponse<T> ErrorResponse(string message, int statusCode)
    {
        var response = new BaseResponse<T>(false, message);
        response.StatusCode = statusCode;
        return response;
    }

    public static BaseResponse<T> ValidationErrorResponse(Dictionary<string, List<string>> validationErrors)
    {
        var errors = validationErrors.SelectMany(x => x.Value).ToList();
        var response = new BaseResponse<T>(false, "Validation failed", default, errors);
        response.StatusCode = 400;
        return response;
    }
}
