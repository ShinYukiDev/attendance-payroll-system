namespace AtendancePayrollSystem.Application.Contracts;

public enum AttendanceOperationStatus
{
    Success,
    Validation,
    Conflict,
    NotFound,
    Failure
}

public sealed class AttendanceOperationResult
{
    public static AttendanceOperationResult Ok(string? message = null) =>
        new(AttendanceOperationStatus.Success, message, []);

    public static AttendanceOperationResult Validation(params string[] errors) =>
        new(AttendanceOperationStatus.Validation, null, errors);

    public static AttendanceOperationResult Conflict(string? message = null) =>
        new(AttendanceOperationStatus.Conflict, message, []);

    public static AttendanceOperationResult NotFound(string? message = null) =>
        new(AttendanceOperationStatus.NotFound, message, []);

    public static AttendanceOperationResult Failure(string? message = null) =>
        new(AttendanceOperationStatus.Failure, message, []);

    private AttendanceOperationResult(AttendanceOperationStatus status, string? message, IReadOnlyList<string> errors)
    {
        Status = status;
        Message = message;
        Errors = errors;
    }

    public AttendanceOperationStatus Status { get; }

    public string? Message { get; }

    public IReadOnlyList<string> Errors { get; }

    public bool IsSuccess => Status == AttendanceOperationStatus.Success;
}

public sealed class AttendanceOperationResult<T>
{
    public static AttendanceOperationResult<T> Ok(T value, string? message = null) =>
        new(AttendanceOperationStatus.Success, value, message, []);

    public static AttendanceOperationResult<T> Validation(params string[] errors) =>
        new(AttendanceOperationStatus.Validation, default, null, errors);

    public static AttendanceOperationResult<T> Conflict(string? message = null) =>
        new(AttendanceOperationStatus.Conflict, default, message, []);

    public static AttendanceOperationResult<T> NotFound(string? message = null) =>
        new(AttendanceOperationStatus.NotFound, default, message, []);

    public static AttendanceOperationResult<T> Failure(string? message = null) =>
        new(AttendanceOperationStatus.Failure, default, message, []);

    private AttendanceOperationResult(AttendanceOperationStatus status, T? value, string? message, IReadOnlyList<string> errors)
    {
        Status = status;
        Value = value;
        Message = message;
        Errors = errors;
    }

    public AttendanceOperationStatus Status { get; }

    public T? Value { get; }

    public string? Message { get; }

    public IReadOnlyList<string> Errors { get; }

    public bool IsSuccess => Status == AttendanceOperationStatus.Success;
}
