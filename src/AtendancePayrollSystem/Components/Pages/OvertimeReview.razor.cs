using System.ComponentModel.DataAnnotations;
using AtendancePayrollSystem.Application.Contracts;
using AtendancePayrollSystem.Application.Queries;
using Microsoft.AspNetCore.Components;

namespace AtendancePayrollSystem.Components.Pages;

public class OvertimeReviewBase : ComponentBase
{
    [Inject] protected OvertimeReviewQueryService QueryService { get; set; } = default!;

    [Inject] protected ILogger<OvertimeReviewBase> Logger { get; set; } = default!;

    protected OvertimeSearchFormModel SearchModel { get; set; } = new();

    protected List<OvertimeReviewDailyRow> DailyRows { get; set; } = [];

    protected OvertimeReviewMonthlySummary? MonthlySummary { get; set; }

    protected string? StatusMessage { get; set; }

    protected string StatusAlertCss { get; set; } = "alert-info";

    protected async Task SearchAsync()
    {
        ClearResult();

        var criteria = new OvertimeSearchCriteria
        {
            EmployeeId = SearchModel.EmployeeId?.Trim() ?? string.Empty,
            TargetMonth = SearchModel.TargetMonth?.Trim() ?? string.Empty
        };

        var result = await QueryService.SearchAsync(criteria);

        if (result.Status == OvertimeReviewStatus.Validation)
        {
            Logger.LogWarning("残業確認画面: 検索条件の検証エラーが発生しました。");
            SetStatus(string.Join(" ", result.Errors), isError: true);
            return;
        }

        if (result.Status == OvertimeReviewStatus.Failure)
        {
            Logger.LogError("残業確認画面: 検索処理に失敗しました。");
            SetStatus(result.Message ?? "残業確認データの取得に失敗しました。", isError: true);
            return;
        }

        DailyRows = result.DailyRows.ToList();
        MonthlySummary = result.MonthlySummary;

        if (DailyRows.Count == 0)
        {
            SetStatus("指定した条件に一致する勤怠データはありません。", isError: false);
            return;
        }

        StatusMessage = null;
    }

    protected static string FormatMinutes(int minutes)
    {
        var hours = minutes / 60;
        var remains = minutes % 60;
        return $"{hours}:{remains:00}";
    }

    private void ClearResult()
    {
        DailyRows = [];
        MonthlySummary = null;
        StatusMessage = null;
        StatusAlertCss = "alert-info";
    }

    private void SetStatus(string message, bool isError)
    {
        StatusMessage = message;
        StatusAlertCss = isError ? "alert-danger" : "alert-info";
    }

    protected sealed class OvertimeSearchFormModel
    {
        [Required]
        [RegularExpression("^[0-9]{7}$", ErrorMessage = "社員IDは半角数字7桁で入力してください。")]
        public string EmployeeId { get; set; } = string.Empty;

        [Required(ErrorMessage = "対象月を入力してください。")]
        public string TargetMonth { get; set; } = string.Empty;
    }
}
