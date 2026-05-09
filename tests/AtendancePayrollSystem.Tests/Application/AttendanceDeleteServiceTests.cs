using AtendancePayrollSystem.Application.Contracts;
using AtendancePayrollSystem.Application.Services;
using AtendancePayrollSystem.Domain.Entities;
using AtendancePayrollSystem.Tests.Infrastructure;

namespace AtendancePayrollSystem.Tests.Application;

public class AttendanceDeleteServiceTests
{
    [Fact]
    public async Task DeleteAsync_削除成功時は監査ログを記録する()
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
            ConcurrencyToken = 1,
            CreatedAtUtc = DateTime.UtcNow,
            UpdatedAtUtc = DateTime.UtcNow,
            CreatedBy = "SYSTEM",
            UpdatedBy = "SYSTEM"
        };
        context.AttendanceRecords.Add(record);
        await context.SaveChangesAsync();

        var service = new AttendanceDeleteService(context, new AttendanceAuditService(context));

        var result = await service.DeleteAsync(record.AttendanceRecordId, 1);

        Assert.True(result.IsSuccess);
        Assert.Equal(0, context.AttendanceRecords.Count());
        Assert.Equal(1, context.AttendanceAuditLogs.Count(x => x.ActionType == "DELETE"));
    }

    [Fact]
    public async Task DeleteAsync_対象なしの場合はNotFoundを返す()
    {
        var (dbContext, connection) = TestDbContextFactory.CreateSqliteContext();
        await using var _ = connection;
        await using var context = dbContext;

        var service = new AttendanceDeleteService(context, new AttendanceAuditService(context));

        var result = await service.DeleteAsync(Guid.NewGuid(), 1);

        Assert.Equal(AttendanceOperationStatus.NotFound, result.Status);
    }

    [Fact]
    public async Task DeleteAsync_競合時はConflictを返す()
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
            ConcurrencyToken = 5,
            CreatedAtUtc = DateTime.UtcNow,
            UpdatedAtUtc = DateTime.UtcNow,
            CreatedBy = "SYSTEM",
            UpdatedBy = "SYSTEM"
        };
        context.AttendanceRecords.Add(record);
        await context.SaveChangesAsync();

        var service = new AttendanceDeleteService(context, new AttendanceAuditService(context));

        var result = await service.DeleteAsync(record.AttendanceRecordId, 4);

        Assert.Equal(AttendanceOperationStatus.Conflict, result.Status);
        Assert.Equal(1, context.AttendanceAuditLogs.Count(x => x.ActionType == "CONFLICT_REJECTED"));
    }
}
