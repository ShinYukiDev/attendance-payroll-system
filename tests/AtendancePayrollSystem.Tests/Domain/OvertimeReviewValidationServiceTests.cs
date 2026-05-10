using AtendancePayrollSystem.Application.Contracts;
using AtendancePayrollSystem.Domain.Services;

namespace AtendancePayrollSystem.Tests.Domain;

public class OvertimeReviewValidationServiceTests
{
    private readonly OvertimeReviewValidationService _service = new();

    [Fact]
    public void Validate_社員ID未入力の場合_エラーを返す()
    {
        var criteria = new OvertimeSearchCriteria
        {
            EmployeeId = string.Empty,
            TargetMonth = "2026-05"
        };

        var errors = _service.Validate(criteria);

        Assert.Contains("社員IDは半角数字7桁で入力してください。", errors);
    }

    [Fact]
    public void Validate_社員ID形式不正の場合_エラーを返す()
    {
        var criteria = new OvertimeSearchCriteria
        {
            EmployeeId = "ABC1234",
            TargetMonth = "2026-05"
        };

        var errors = _service.Validate(criteria);

        Assert.Contains("社員IDは半角数字7桁で入力してください。", errors);
    }

    [Fact]
    public void Validate_対象月未入力の場合_エラーを返す()
    {
        var criteria = new OvertimeSearchCriteria
        {
            EmployeeId = "1234567",
            TargetMonth = string.Empty
        };

        var errors = _service.Validate(criteria);

        Assert.Contains("対象月を入力してください。", errors);
    }

    [Fact]
    public void Validate_対象月形式不正の場合_エラーを返す()
    {
        var criteria = new OvertimeSearchCriteria
        {
            EmployeeId = "1234567",
            TargetMonth = "2026/05"
        };

        var errors = _service.Validate(criteria);

        Assert.Contains("対象月の形式が正しくありません。", errors);
    }

    [Fact]
    public void TryGetMonthRange_正しい年月の場合_月初と翌月初を返す()
    {
        var result = _service.TryGetMonthRange("2026-05", out var start, out var endExclusive);

        Assert.True(result);
        Assert.Equal(new DateOnly(2026, 5, 1), start);
        Assert.Equal(new DateOnly(2026, 6, 1), endExclusive);
    }
}
