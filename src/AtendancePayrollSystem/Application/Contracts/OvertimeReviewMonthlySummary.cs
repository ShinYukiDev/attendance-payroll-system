namespace AtendancePayrollSystem.Application.Contracts;

public sealed class OvertimeReviewMonthlySummary
{
    public int TotalWorkMinutes { get; set; }

    public int TotalStandardMinutes { get; set; }

    public int TotalOvertimeMinutes { get; set; }

    public int RowCount { get; set; }
}
