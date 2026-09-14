namespace CareConnect.DTOs.Errors;

/// <summary>A referenced resource doesn't exist. Maps to HTTP 404.</summary>
public sealed class NotFoundException : Exception
{
    public NotFoundException(string message) : base(message)
    {
    }
}
