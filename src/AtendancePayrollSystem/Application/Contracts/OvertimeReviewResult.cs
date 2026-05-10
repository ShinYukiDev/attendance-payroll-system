namespace AtendancePayrollSystem.Application.Contracts;

public enum OvertimeReviewStatus
{
    Success,
    Validation,
    Failure
}

public sealed class OvertimeReviewResult
{
    public static OvertimeReviewResult Success(
        IReadOnlyList<OvertimeReviewDailyRow> dailyRows,
        OvertimeReviewMonthlySummary? monthlySummary) =>
        new(OvertimeReviewStatus.Success, dailyRows, monthlySummary, null, []);

    public static OvertimeReviewResult Validation(params string[] errors) =>
        new(OvertimeReviewStatus.Validation, [], null, null, errors);

    public static OvertimeReviewResult Failure(string message) =>
        new(OvertimeReviewStatus.Failure, [], null, message, []);

    private OvertimeReviewResult(
        OvertimeReviewStatus status,
        IReadOnlyList<OvertimeReviewDailyRow> dailyRows,
        OvertimeReviewMonthlySummary? monthlySummary,
        string? message,
        IReadOnlyList<string> errors)
    {
        Status = status;
        DailyRows = dailyRows;
        MonthlySummary = monthlySummary;
        Message = message;
        Errors = errors;
    }

    public OvertimeReviewStatus Status { get; }

    public IReadOnlyList<OvertimeReviewDailyRow> DailyRows { get; }

    public OvertimeReviewMonthlySummary? MonthlySummary { get; }

    public string? Message { get; }

    public IReadOnlyList<string> Errors { get; }

    public bool IsSuccess => Status == OvertimeReviewStatus.Success;
}
