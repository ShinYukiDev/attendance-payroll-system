using AtendancePayrollSystem.Application.Contracts;
using AtendancePayrollSystem.Application.Queries;
using AtendancePayrollSystem.Domain.Entities;
using AtendancePayrollSystem.Domain.Services;
using AtendancePayrollSystem.Tests.Infrastructure;
using Microsoft.Extensions.Logging.Abstractions;

namespace AtendancePayrollSystem.Tests.Application;

public class OvertimeReviewQueryServiceTests
{
    [Fact]
    public async Task SearchAsync_検索成功時に日別行を取得できる()
    {
        var (dbContext, connection) = TestDbContextFactory.CreateSqliteContext();
        await using var _ = connection;
        await using var context = dbContext;

        context.AttendanceRecords.AddRange(
            BuildRecord("1234567", new DateOnly(2026, 5, 8), 495),
            BuildRecord("1234567", new DateOnly(2026, 5, 9), 540),
            BuildRecord("7654321", new DateOnly(2026, 5, 9), 600));
        await context.SaveChangesAsync();

        var service = BuildService(context);

        var result = await service.SearchAsync(new OvertimeSearchCriteria
        {
            EmployeeId = "1234567",
            TargetMonth = "2026-05"
        });

        Assert.True(result.IsSuccess);
        Assert.Equal(2, result.DailyRows.Count);
        Assert.Equal(new DateOnly(2026, 5, 8), result.DailyRows[0].WorkDate);
        Assert.Equal(15, result.DailyRows[0].OvertimeMinutes);
        Assert.Equal(60, result.DailyRows[1].OvertimeMinutes);
    }

    [Fact]
    public async Task SearchAsync_条件変更後に再検索結果が置き換わる()
    {
        var (dbContext, connection) = TestDbContextFactory.CreateSqliteContext();
        await using var _ = connection;
        await using var context = dbContext;

        context.AttendanceRecords.AddRange(
            BuildRecord("1234567", new DateOnly(2026, 5, 1), 600),
            BuildRecord("1234567", new DateOnly(2026, 6, 1), 510));
        await context.SaveChangesAsync();

        var service = BuildService(context);

        var mayResult = await service.SearchAsync(new OvertimeSearchCriteria
        {
            EmployeeId = "1234567",
            TargetMonth = "2026-05"
        });

        var juneResult = await service.SearchAsync(new OvertimeSearchCriteria
        {
            EmployeeId = "1234567",
            TargetMonth = "2026-06"
        });

        Assert.Single(mayResult.DailyRows);
        Assert.Equal(new DateOnly(2026, 5, 1), mayResult.DailyRows[0].WorkDate);

        Assert.Single(juneResult.DailyRows);
        Assert.Equal(new DateOnly(2026, 6, 1), juneResult.DailyRows[0].WorkDate);
    }

    [Fact]
    public async Task SearchAsync_月合計行の集計値を返す()
    {
        var (dbContext, connection) = TestDbContextFactory.CreateSqliteContext();
        await using var _ = connection;
        await using var context = dbContext;

        context.AttendanceRecords.AddRange(
            BuildRecord("1234567", new DateOnly(2026, 5, 10), 480),
            BuildRecord("1234567", new DateOnly(2026, 5, 11), 600));
        await context.SaveChangesAsync();

        var service = BuildService(context);

        var result = await service.SearchAsync(new OvertimeSearchCriteria
        {
            EmployeeId = "1234567",
            TargetMonth = "2026-05"
        });

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.MonthlySummary);
        Assert.Equal(1080, result.MonthlySummary!.TotalWorkMinutes);
        Assert.Equal(960, result.MonthlySummary.TotalStandardMinutes);
        Assert.Equal(120, result.MonthlySummary.TotalOvertimeMinutes);
        Assert.Equal(2, result.MonthlySummary.RowCount);
    }

    [Fact]
    public async Task SearchAsync_検索結果0件時は空結果を返す()
    {
        var (dbContext, connection) = TestDbContextFactory.CreateSqliteContext();
        await using var _ = connection;
        await using var context = dbContext;

        context.AttendanceRecords.Add(BuildRecord("9999999", new DateOnly(2026, 5, 10), 540));
        await context.SaveChangesAsync();

        var service = BuildService(context);

        var result = await service.SearchAsync(new OvertimeSearchCriteria
        {
            EmployeeId = "1234567",
            TargetMonth = "2026-05"
        });

        Assert.True(result.IsSuccess);
        Assert.Empty(result.DailyRows);
        Assert.Null(result.MonthlySummary);
    }

    private static OvertimeReviewQueryService BuildService(AtendancePayrollSystem.Infrastructure.AttendanceDbContext context)
    {
        var calculationService = new OvertimeCalculationService();
        var validationService = new OvertimeReviewValidationService();
        return new OvertimeReviewQueryService(
            context,
            calculationService,
            validationService,
            NullLogger<OvertimeReviewQueryService>.Instance);
    }

    private static AttendanceRecord BuildRecord(string employeeId, DateOnly workDate, int workMinutes)
    {
        return new AttendanceRecord
        {
            AttendanceRecordId = Guid.NewGuid(),
            EmployeeId = employeeId,
            WorkDate = workDate,
            StartTime = new TimeOnly(9, 0),
            EndTime = new TimeOnly(18, 0),
            BreakMinutes = 60,
            WorkMinutes = workMinutes,
            ConcurrencyToken = 1,
            CreatedAtUtc = DateTime.UtcNow,
            UpdatedAtUtc = DateTime.UtcNow,
            CreatedBy = "SYSTEM",
            UpdatedBy = "SYSTEM"
        };
    }
}
