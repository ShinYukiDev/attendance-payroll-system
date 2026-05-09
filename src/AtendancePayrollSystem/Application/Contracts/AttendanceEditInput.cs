namespace AtendancePayrollSystem.Application.Contracts;

public sealed class AttendanceEditInput
{
    public string EmployeeId { get; set; } = string.Empty;

    public DateOnly WorkDate { get; set; }

    public TimeOnly StartTime { get; set; }

    public TimeOnly EndTime { get; set; }

    public int BreakMinutes { get; set; }
}
