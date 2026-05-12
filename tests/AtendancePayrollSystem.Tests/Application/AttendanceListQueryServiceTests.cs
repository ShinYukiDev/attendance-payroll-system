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

    [Fact]
    public async Task GetListAsync_社員IDと勤務日で昇順ソートされる()
    {
        var (dbContext, connection) = TestDbContextFactory.CreateSqliteContext();
        await using var _ = connection;
        await using var context = dbContext;

        context.AttendanceRecords.AddRange(
            BuildRecord("0000002", new DateOnly(2026, 5, 8)),
            BuildRecord("0000001", new DateOnly(2026, 5, 10)),
            BuildRecord("0000001", new DateOnly(2026, 5, 8)),
            BuildRecord("0000003", new DateOnly(2026, 5, 1)));
        await context.SaveChangesAsync();

        var service = new AttendanceListQueryService(context);

        var items = await service.GetListAsync();

        Assert.Equal(4, items.Count);
        Assert.Collection(
            items,
            x =>
            {
                Assert.Equal("0000001", x.EmployeeId);
                Assert.Equal(new DateOnly(2026, 5, 8), x.WorkDate);
            },
            x =>
            {
                Assert.Equal("0000001", x.EmployeeId);
                Assert.Equal(new DateOnly(2026, 5, 10), x.WorkDate);
            },
            x =>
            {
                Assert.Equal("0000002", x.EmployeeId);
                Assert.Equal(new DateOnly(2026, 5, 8), x.WorkDate);
            },
            x =>
            {
                Assert.Equal("0000003", x.EmployeeId);
                Assert.Equal(new DateOnly(2026, 5, 1), x.WorkDate);
            });
    }

    private static AttendanceRecord BuildRecord(string employeeId, DateOnly workDate)
    {
        return new AttendanceRecord
        {
            AttendanceRecordId = Guid.NewGuid(),
            EmployeeId = employeeId,
            WorkDate = workDate,
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
    }
}
