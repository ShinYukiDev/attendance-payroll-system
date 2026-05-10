using AtendancePayrollSystem.Application.Contracts;
using AtendancePayrollSystem.Domain.Services;

namespace AtendancePayrollSystem.Tests.Domain;

public class OvertimeCalculationServiceTests
{
    private readonly OvertimeCalculationService _service = new();

    [Fact]
    public void CalculateOvertimeMinutes_8時間超過の場合_超過分を返す()
    {
        var overtime = _service.CalculateOvertimeMinutes(510);

        Assert.Equal(30, overtime);
    }

    [Fact]
    public void CalculateOvertimeMinutes_8時間以下の場合_0を返す()
    {
        var overtime = _service.CalculateOvertimeMinutes(480);

        Assert.Equal(0, overtime);
    }

    [Fact]
    public void BuildMonthlySummary_合計値を正しく返す()
    {
        var rows = new List<OvertimeReviewDailyRow>
        {
            new()
            {
                WorkDate = new DateOnly(2026, 5, 1),
                WorkMinutes = 480,
                StandardMinutes = 480,
                OvertimeMinutes = 0
            },
            new()
            {
                WorkDate = new DateOnly(2026, 5, 2),
                WorkMinutes = 540,
                StandardMinutes = 480,
                OvertimeMinutes = 60
            }
        };

        var summary = _service.BuildMonthlySummary(rows);

        Assert.NotNull(summary);
        Assert.Equal(1020, summary!.TotalWorkMinutes);
        Assert.Equal(960, summary.TotalStandardMinutes);
        Assert.Equal(60, summary.TotalOvertimeMinutes);
        Assert.Equal(2, summary.RowCount);
    }

    [Fact]
    public void BuildMonthlySummary_日別行0件の場合_nullを返す()
    {
        var summary = _service.BuildMonthlySummary([]);

        Assert.Null(summary);
    }
}
