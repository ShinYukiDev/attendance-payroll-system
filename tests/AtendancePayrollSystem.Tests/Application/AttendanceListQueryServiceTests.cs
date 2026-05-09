using AtendancePayrollSystem.Application.Queries;
using AtendancePayrollSystem.Domain.Entities;
using AtendancePayrollSystem.Tests.Infrastructure;

namespace AtendancePayrollSystem.Tests.Application;

public class AttendanceListQueryServiceTests
{
    [Fact]
    public async Task GetListAsync_一覧表示項目を取得できる()
    {
        var (dbContext, connection) = TestDbContextFactory.CreateSqliteContext();
        await using var _ = connection;
        await using var context = dbContext;

        context.AttendanceRecords.Add(new AttendanceRecord
        {
            AttendanceRecordId = Guid.NewGuid(),
            EmployeeId = "7654321",
            WorkDate = new DateOnly(2026, 5, 8),
            StartTime = new TimeOnly(8, 30),
            EndTime = new TimeOnly(17, 30),
            BreakMinutes = 45,
            WorkMinutes = 495,
            ConcurrencyToken = 1,
            CreatedAtUtc = DateTime.UtcNow,
            UpdatedAtUtc = DateTime.UtcNow,
            CreatedBy = "SYSTEM",
            UpdatedBy = "SYSTEM"
        });
        await context.SaveChangesAsync();

        var service = new AttendanceListQueryService(context);

        var items = await service.GetListAsync();

        Assert.Single(items);
        var item = items[0];
        Assert.Equal("7654321", item.EmployeeId);
        Assert.Equal(new DateOnly(2026, 5, 8), item.WorkDate);
        Assert.Equal(new TimeOnly(8, 30), item.StartTime);
        Assert.Equal(new TimeOnly(17, 30), item.EndTime);
        Assert.Equal(45, item.BreakMinutes);
        Assert.Equal(495, item.WorkMinutes);
    }
}
