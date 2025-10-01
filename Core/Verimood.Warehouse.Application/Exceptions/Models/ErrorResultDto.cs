using System.Text.Json.Serialization;

namespace Verimood.Warehouse.Application.Exceptions.Models;

public class ErrorResultDto
{
    public List<string> Messages { get; set; } = new();
    public string ThrowMessage { get; set; } = default!;

    [JsonIgnore]
    public string? Source { get; set; }

    public string? Exception { get; set; }
    public string? ErrorId { get; set; }

    [JsonIgnore]
    public string? SupportMessage { get; set; }

    [JsonIgnore]
    public int StatusCode { get; set; }
}
