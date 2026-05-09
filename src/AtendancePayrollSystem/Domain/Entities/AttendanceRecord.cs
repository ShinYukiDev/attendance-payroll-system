namespace AtendancePayrollSystem.Domain.Entities;

public sealed class AttendanceRecord
{
    public Guid AttendanceRecordId { get; set; }

    public string EmployeeId { get; set; } = string.Empty;

    public DateOnly WorkDate { get; set; }

    public TimeOnly StartTime { get; set; }

    public TimeOnly EndTime { get; set; }

    public int BreakMinutes { get; set; }

    public int WorkMinutes { get; set; }

    public int ConcurrencyToken { get; set; }

    public DateTime CreatedAtUtc { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime UpdatedAtUtc { get; set; }

    public string? UpdatedBy { get; set; }
}
