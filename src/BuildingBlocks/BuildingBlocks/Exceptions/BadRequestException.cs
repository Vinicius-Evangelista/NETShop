namespace BuildingBlocks.Exceptions;

public class BadRequestException : Exception
{
    public BadRequestException(string message)
        : base(message: message) { }

    public BadRequestException(string message, string details)
        : base(message: message) => Details = details;

    public string? Details { get; set; }
}
