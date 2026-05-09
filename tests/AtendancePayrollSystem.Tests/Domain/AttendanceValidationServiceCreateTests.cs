using AtendancePayrollSystem.Application.Contracts;
using AtendancePayrollSystem.Domain.Services;

namespace AtendancePayrollSystem.Tests.Domain;

public class AttendanceValidationServiceCreateTests
{
    private readonly AttendanceValidationService _service = new();

    [Fact]
    public void Validate_社員IDが不正の場合_エラーを返す()
    {
        var input = BuildInput(employeeId: "A123");

        var errors = _service.Validate(input);

        Assert.Contains("社員IDは半角数字7桁で入力してください。", errors);
    }

    [Fact]
    public void Validate_退勤時刻が出勤時刻以前の場合_エラーを返す()
    {
        var input = BuildInput(start: new TimeOnly(9, 0), end: new TimeOnly(9, 0));

        var errors = _service.Validate(input);

        Assert.Contains("退勤時刻は出勤時刻より後にしてください。", errors);
    }

    [Fact]
    public void Validate_休憩時間が勤務時間以上の場合_エラーを返す()
    {
        var input = BuildInput(start: new TimeOnly(9, 0), end: new TimeOnly(10, 0), breakMinutes: 60);

        var errors = _service.Validate(input);

        Assert.Contains("休憩時間は勤務時間未満で入力してください。", errors);
    }

    private static AttendanceEditInput BuildInput(
        string employeeId = "1234567",
        TimeOnly? start = null,
        TimeOnly? end = null,
        int breakMinutes = 60)
    {
        return new AttendanceEditInput
        {
            EmployeeId = employeeId,
            WorkDate = new DateOnly(2026, 5, 9),
            StartTime = start ?? new TimeOnly(9, 0),
            EndTime = end ?? new TimeOnly(18, 0),
            BreakMinutes = breakMinutes
        };
    }
}
