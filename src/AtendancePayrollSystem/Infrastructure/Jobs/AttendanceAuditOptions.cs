namespace AtendancePayrollSystem.Infrastructure.Jobs;

public sealed class AttendanceAuditOptions
{
    public int RetentionDays { get; set; } = 90;
}
