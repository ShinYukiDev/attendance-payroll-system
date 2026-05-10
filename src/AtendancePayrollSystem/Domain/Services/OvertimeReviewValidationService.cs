using System.Globalization;
using System.Text.RegularExpressions;
using AtendancePayrollSystem.Application.Contracts;

namespace AtendancePayrollSystem.Domain.Services;

public sealed class OvertimeReviewValidationService
{
    private static readonly Regex EmployeeIdPattern = new("^[0-9]{7}$", RegexOptions.Compiled);

    public IReadOnlyList<string> Validate(OvertimeSearchCriteria criteria)
    {
        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(criteria.EmployeeId) ||
            !EmployeeIdPattern.IsMatch(criteria.EmployeeId.Trim()))
        {
            errors.Add("社員IDは半角数字7桁で入力してください。");
        }

        if (string.IsNullOrWhiteSpace(criteria.TargetMonth))
        {
            errors.Add("対象月を入力してください。");
            return errors;
        }

        if (!TryGetMonthRange(criteria.TargetMonth, out _, out _))
        {
            errors.Add("対象月の形式が正しくありません。");
        }

        return errors;
    }

    public bool TryGetMonthRange(string targetMonth, out DateOnly monthStart, out DateOnly monthEndExclusive)
    {
        monthStart = default;
        monthEndExclusive = default;

        if (!DateTime.TryParseExact(
                targetMonth,
                "yyyy-MM",
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out var parsed))
        {
            return false;
        }

        monthStart = new DateOnly(parsed.Year, parsed.Month, 1);
        monthEndExclusive = monthStart.AddMonths(1);
        return true;
    }
}
