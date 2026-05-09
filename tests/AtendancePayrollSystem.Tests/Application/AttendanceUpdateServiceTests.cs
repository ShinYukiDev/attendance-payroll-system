using AtendancePayrollSystem.Application.Contracts;
using AtendancePayrollSystem.Application.Services;
using AtendancePayrollSystem.Domain.Entities;
using AtendancePayrollSystem.Domain.Services;
using AtendancePayrollSystem.Tests.Infrastructure;

namespace AtendancePayrollSystem.Tests.Application;

public class AttendanceUpdateServiceTests
{
    [Fact]
    public async Task UpdateAsync_更新成功時は実働時間再計算とトークン更新を行う()
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

        var auditService = new AttendanceAuditService(context);
        var validationService = new AttendanceValidationService();
        var service = new AttendanceUpdateService(context, validationService, auditService);

        var input = new AttendanceEditInput
        {
            EmployeeId = "1234567",
            WorkDate = new DateOnly(2026, 5, 9),
            StartTime = new TimeOnly(9, 0),
            EndTime = new TimeOnly(19, 0),
            BreakMinutes = 30
        };

        var result = await service.UpdateAsync(record.AttendanceRecordId, 1, input);

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(570, result.Value!.WorkMinutes);
        Assert.Equal(2, result.Value.ConcurrencyToken);

        var auditCount = context.AttendanceAuditLogs.Count(x => x.ActionType == "UPDATE");
        Assert.Equal(1, auditCount);
    }
}
