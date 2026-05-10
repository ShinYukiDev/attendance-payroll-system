using AtendancePayrollSystem.Application.Contracts;
using AtendancePayrollSystem.Domain.Services;
using AtendancePayrollSystem.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace AtendancePayrollSystem.Application.Queries;

public sealed class OvertimeReviewQueryService(
    AttendanceDbContext dbContext,
    OvertimeCalculationService calculationService,
    OvertimeReviewValidationService validationService,
    ILogger<OvertimeReviewQueryService> logger)
{
    public async Task<OvertimeReviewResult> SearchAsync(
        OvertimeSearchCriteria criteria,
        CancellationToken cancellationToken = default)
    {
        var errors = validationService.Validate(criteria);
        if (errors.Count > 0)
        {
            return OvertimeReviewResult.Validation(errors.ToArray());
        }

        if (!validationService.TryGetMonthRange(criteria.TargetMonth, out var monthStart, out var monthEndExclusive))
        {
            return OvertimeReviewResult.Validation("対象月の形式が正しくありません。");
        }

        try
        {
            var employeeId = criteria.EmployeeId.Trim();
            var records = await dbContext.AttendanceRecords
                .AsNoTracking()
                .Where(x => x.EmployeeId == employeeId)
                .Where(x => x.WorkDate >= monthStart && x.WorkDate < monthEndExclusive)
                .OrderBy(x => x.WorkDate)
                .Select(x => new { x.WorkDate, x.WorkMinutes })
                .ToListAsync(cancellationToken);

            var dailyRows = records
                .Select(x => calculationService.CreateDailyRow(x.WorkDate, x.WorkMinutes))
                .ToList();

            var summary = calculationService.BuildMonthlySummary(dailyRows);
            return OvertimeReviewResult.Success(dailyRows, summary);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "残業確認データの取得に失敗しました。");
            return OvertimeReviewResult.Failure("残業確認データの取得に失敗しました。");
        }
    }
}
