using AtendancePayrollSystem.Application.Contracts;
using AtendancePayrollSystem.Application.Services;
using AtendancePayrollSystem.Domain.Entities;
using AtendancePayrollSystem.Domain.Services;
using AtendancePayrollSystem.Tests.Infrastructure;

namespace AtendancePayrollSystem.Tests.Application;

public class AttendanceCreateServiceTests
{
    [Fact]
    public async Task CreateAsync_登録成功時は実働時間を算出して保存する()
    {
        var (dbContext, connection) = TestDbContextFactory.CreateSqliteContext();
        await using var _ = connection;
        await using var context = dbContext;

        var auditService = new AttendanceAuditService(context);
        var validationService = new AttendanceValidationService();
        var service = new AttendanceCreateService(context, validationService, auditService);

        var input = new AttendanceEditInput
        {
            EmployeeId = "1234567",
            WorkDate = new DateOnly(2026, 5, 9),
            StartTime = new TimeOnly(9, 0),
            EndTime = new TimeOnly(18, 0),
            BreakMinutes = 60
        };

        var result = await service.CreateAsync(input);

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(480, result.Value!.WorkMinutes);

        var saved = await context.AttendanceRecords.FindAsync(result.Value.AttendanceRecordId);
        Assert.NotNull(saved);
        Assert.Equal(1, saved!.ConcurrencyToken);

        var auditCount = context.AttendanceAuditLogs.Count();
        Assert.Equal(1, auditCount);
    }
}
