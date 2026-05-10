using AtendancePayrollSystem.Application.Contracts;

namespace AtendancePayrollSystem.Domain.Services;

public sealed class OvertimeCalculationService
{
    public const int StandardMinutesPerDay = 8 * 60;

    public int CalculateOvertimeMinutes(int workMinutes)
    {
        return Math.Max(workMinutes - StandardMinutesPerDay, 0);
    }

    public OvertimeReviewDailyRow CreateDailyRow(DateOnly workDate, int workMinutes)
    {
        return new OvertimeReviewDailyRow
        {
            WorkDate = workDate,
            WorkMinutes = workMinutes,
            StandardMinutes = StandardMinutesPerDay,
            OvertimeMinutes = CalculateOvertimeMinutes(workMinutes)
        };
    }

    public OvertimeReviewMonthlySummary? BuildMonthlySummary(IReadOnlyList<OvertimeReviewDailyRow> dailyRows)
    {
        if (dailyRows.Count == 0)
        {
            return null;
        }

        return new OvertimeReviewMonthlySummary
        {
            TotalWorkMinutes = dailyRows.Sum(x => x.WorkMinutes),
            TotalStandardMinutes = dailyRows.Sum(x => x.StandardMinutes),
            TotalOvertimeMinutes = dailyRows.Sum(x => x.OvertimeMinutes),
            RowCount = dailyRows.Count
        };
    }
}
