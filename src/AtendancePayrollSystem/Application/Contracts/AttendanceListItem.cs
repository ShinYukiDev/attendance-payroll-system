namespace AtendancePayrollSystem.Application.Contracts;

public sealed class AttendanceListItem
{
    public Guid AttendanceRecordId { get; set; }

    public string EmployeeId { get; set; } = string.Empty;

    public DateOnly WorkDate { get; set; }

    public TimeOnly StartTime { get; set; }

    public TimeOnly EndTime { get; set; }

    public int BreakMinutes { get; set; }

    public int WorkMinutes { get; set; }

    public int ConcurrencyToken { get; set; }
}
