using AtendancePayrollSystem.Domain.Entities;
using AtendancePayrollSystem.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace AtendancePayrollSystem.Application.Services;

public sealed class AttendanceAuditService(AttendanceDbContext dbContext)
{
    public async Task WriteAsync(
        string actionType,
        Guid recordId,
        string employeeId,
        DateOnly workDate,
        string changedFields,
        string result,
        string? reasonCode = null,
        CancellationToken cancellationToken = default)
    {
        var log = new AttendanceAuditLog
        {
            AuditId = Guid.NewGuid(),
            OccurredAtUtc = DateTime.UtcNow,
            ActionType = actionType,
            ActorId = "SYSTEM",
            TargetRecordId = recordId,
            TargetEmployeeIdMasked = MaskEmployeeId(employeeId),
            TargetWorkDate = workDate,
            ChangedFields = changedFields,
            Result = result,
            ReasonCode = reasonCode
        };

        dbContext.AttendanceAuditLogs.Add(log);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task WriteConflictAsync(
        Guid recordId,
        string employeeId,
        DateOnly workDate,
        string reasonCode,
        CancellationToken cancellationToken = default)
    {
        var log = new AttendanceAuditLog
        {
            AuditId = Guid.NewGuid(),
            OccurredAtUtc = DateTime.UtcNow,
            ActionType = "CONFLICT_REJECTED",
            ActorId = "SYSTEM",
            TargetRecordId = recordId,
            TargetEmployeeIdMasked = MaskEmployeeId(employeeId),
            TargetWorkDate = workDate,
            ChangedFields = "",
            Result = "FAILURE",
            ReasonCode = reasonCode
        };

        dbContext.AttendanceAuditLogs.Add(log);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<int> CleanupExpiredLogsAsync(int retentionDays, CancellationToken cancellationToken = default)
    {
        var threshold = DateTime.UtcNow.AddDays(-retentionDays);
        var targets = await dbContext.AttendanceAuditLogs
            .Where(x => x.OccurredAtUtc < threshold)
            .ToListAsync(cancellationToken);

        if (targets.Count == 0)
        {
            return 0;
        }

        dbContext.AttendanceAuditLogs.RemoveRange(targets);
        await dbContext.SaveChangesAsync(cancellationToken);
        return targets.Count;
    }

    private static string MaskEmployeeId(string employeeId)
    {
        if (string.IsNullOrWhiteSpace(employeeId) || employeeId.Length < 3)
        {
            return "__";
        }

        return $"{employeeId[..2]}_{employeeId[^1]}";
    }
}
