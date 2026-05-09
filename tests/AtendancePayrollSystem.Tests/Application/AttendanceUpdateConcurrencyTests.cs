using AtendancePayrollSystem.Application.Contracts;
using AtendancePayrollSystem.Application.Services;
using AtendancePayrollSystem.Domain.Entities;
using AtendancePayrollSystem.Domain.Services;
using AtendancePayrollSystem.Tests.Infrastructure;

namespace AtendancePayrollSystem.Tests.Application;

public class AttendanceUpdateConcurrencyTests
{
    [Fact]
    public async Task UpdateAsync_トークン不一致時は競合として保存拒否する()
    {
        var (dbContext, connection) = TestDbContextFactory.CreateSqliteContext();
        await using var _ = connection;
        await using var context = dbContext;

        var record = new AttendanceRecord
        {
            AttendanceRecordId = Guid.NewGuid(),
            EmployeeId = "1234567",
            WorkDate = new DateOnly(2026, 5, 9),
            StartTime = new TimeOnly(9, 0),
            EndTime = new TimeOnly(18, 0),
            BreakMinutes = 60,
            WorkMinutes = 480,
            ConcurrencyToken = 3,
            CreatedAtUtc = DateTime.UtcNow,
            UpdatedAtUtc = DateTime.UtcNow,
            CreatedBy = "SYSTEM",
            UpdatedBy = "SYSTEM"
        };
        context.AttendanceRecords.Add(record);
        await context.SaveChangesAsync();

        var service = new AttendanceUpdateService(
            context,
            new AttendanceValidationService(),
            new AttendanceAuditService(context));

        var input = new AttendanceEditInput
        {
            EmployeeId = "1234567",
            WorkDate = new DateOnly(2026, 5, 9),
            StartTime = new TimeOnly(9, 0),
            EndTime = new TimeOnly(18, 30),
            BreakMinutes = 60
        };

        var result = await service.UpdateAsync(record.AttendanceRecordId, 2, input);

        Assert.Equal(AttendanceOperationStatus.Conflict, result.Status);
        Assert.False(result.IsSuccess);

        var conflictAuditCount = context.AttendanceAuditLogs.Count(x => x.ActionType == "CONFLICT_REJECTED");
        Assert.Equal(1, conflictAuditCount);
    }
}
