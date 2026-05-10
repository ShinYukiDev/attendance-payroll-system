namespace AtendancePayrollSystem.Application.Contracts;

public sealed class OvertimeReviewDailyRow
{
    public DateOnly WorkDate { get; set; }

    public int WorkMinutes { get; set; }

    public int StandardMinutes { get; set; }

    public int OvertimeMinutes { get; set; }
}
