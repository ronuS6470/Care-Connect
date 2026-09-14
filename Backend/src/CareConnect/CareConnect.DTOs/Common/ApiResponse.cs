namespace CareConnect.DTOs.Common;

/// <summary>
/// Uniform envelope for application/business error responses (validation failures, conflicts,
/// unhandled exceptions). Success responses should return the resource DTO directly, unwrapped —
/// wrapping every 200/201 in this wastes the REST status code's meaning and forces every client
/// to unwrap a "Data" property for no benefit.
/// </summary>
public sealed class ApiResponse<T>
{
    public required bool Success { get; init; }

    public string? Message { get; init; }

    public T? Data { get; init; }

    public IReadOnlyList<string>? Errors { get; init; }

    public static ApiResponse<T> SuccessResponse(T data, string? message = null) =>
        new() { Success = true, Data = data, Message = message };

    public static ApiResponse<T> Fail(string message, IReadOnlyList<string>? errors = null) =>
        new() { Success = false, Message = message, Errors = errors };
}
