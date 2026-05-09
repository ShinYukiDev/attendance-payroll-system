namespace AtendancePayrollSystem.Domain.Entities;

public sealed class AttendanceAuditLog
{
    public Guid AuditId { get; set; }

    public DateTime OccurredAtUtc { get; set; }

    public string ActionType { get; set; } = string.Empty;

    public string? ActorId { get; set; }

    public Guid TargetRecordId { get; set; }

    public string TargetEmployeeIdMasked { get; set; } = string.Empty;

    public DateOnly TargetWorkDate { get; set; }

    public string ChangedFields { get; set; } = string.Empty;

    public string Result { get; set; } = string.Empty;

    public string? ReasonCode { get; set; }
}
