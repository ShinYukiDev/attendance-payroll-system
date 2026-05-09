using AtendancePayrollSystem.Application.Contracts;
using AtendancePayrollSystem.Domain.Services;

namespace AtendancePayrollSystem.Tests.Domain;

public class AttendanceValidationServiceEdgeCaseTests
{
    private readonly AttendanceValidationService _service = new();

    [Fact]
    public void Validate_休憩時間が負数の場合_エラーを返す()
    {
        var input = new AttendanceEditInput
        {
            EmployeeId = "1234567",
            WorkDate = new DateOnly(2026, 5, 9),
            StartTime = new TimeOnly(9, 0),
            EndTime = new TimeOnly(18, 0),
            BreakMinutes = -1
        };

        var errors = _service.Validate(input);

        Assert.Contains("休憩時間は勤務時間未満で入力してください。", errors);
    }

    [Fact]
    public void Validate_実働時間が0分以下の場合_エラーを返す()
    {
        var input = new AttendanceEditInput
        {
            EmployeeId = "1234567",
            WorkDate = new DateOnly(2026, 5, 9),
            StartTime = new TimeOnly(9, 0),
            EndTime = new TimeOnly(9, 1),
            BreakMinutes = 1
        };

        var errors = _service.Validate(input);

        Assert.Contains("実働時間は1分以上にしてください。", errors);
    }

    [Fact]
    public void CalculateWorkMinutes_境界値でも計算できる()
    {
        var input = new AttendanceEditInput
        {
            EmployeeId = "1234567",
            WorkDate = new DateOnly(2026, 5, 9),
            StartTime = new TimeOnly(0, 0),
            EndTime = new TimeOnly(23, 59),
            BreakMinutes = 30
        };

        var workMinutes = _service.CalculateWorkMinutes(input);

        Assert.Equal(1409, workMinutes);
    }
}
