using System.Text.RegularExpressions;
using AtendancePayrollSystem.Application.Contracts;

namespace AtendancePayrollSystem.Domain.Services;

public sealed class AttendanceValidationService
{
    private static readonly Regex EmployeeIdPattern = new("^[0-9]{7}$", RegexOptions.Compiled);

    public IReadOnlyList<string> Validate(AttendanceEditInput input)
    {
        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(input.EmployeeId) || !EmployeeIdPattern.IsMatch(input.EmployeeId))
        {
            errors.Add("社員IDは半角数字7桁で入力してください。");
        }

        if (input.EndTime <= input.StartTime)
        {
            errors.Add("退勤時刻は出勤時刻より後にしてください。");
        }

        var totalMinutes = (int)(input.EndTime - input.StartTime).TotalMinutes;
        if (input.BreakMinutes < 0 || input.BreakMinutes >= totalMinutes)
        {
            errors.Add("休憩時間は勤務時間未満で入力してください。");
        }

        if (totalMinutes - input.BreakMinutes <= 0)
        {
            errors.Add("実働時間は1分以上にしてください。");
        }

        return errors;
    }

    public int CalculateWorkMinutes(AttendanceEditInput input)
    {
        var totalMinutes = (int)(input.EndTime - input.StartTime).TotalMinutes;
        return totalMinutes - input.BreakMinutes;
    }
}
