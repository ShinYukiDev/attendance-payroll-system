using System.ComponentModel.DataAnnotations;
using AtendancePayrollSystem.Application.Contracts;
using AtendancePayrollSystem.Application.Queries;
using AtendancePayrollSystem.Application.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace AtendancePayrollSystem.Components.Pages;

public class AttendanceInputBase : ComponentBase
{
    [Inject] protected AttendanceCreateService CreateService { get; set; } = default!;

    [Inject] protected AttendanceUpdateService UpdateService { get; set; } = default!;

    [Inject] protected AttendanceDeleteService DeleteService { get; set; } = default!;

    [Inject] protected AttendanceListQueryService ListQueryService { get; set; } = default!;

    [Inject] protected IJSRuntime JSRuntime { get; set; } = default!;

    protected AttendanceFormModel InputModel { get; set; } = AttendanceFormModel.CreateDefault();

    protected List<AttendanceListItem> Items { get; set; } = [];

    protected string? StatusMessage { get; set; }

    protected string StatusAlertCss { get; set; } = "alert-info";

    protected Guid? EditingRecordId { get; set; }

    protected int EditingConcurrencyToken { get; set; }

    protected bool IsEditMode => EditingRecordId.HasValue;

    protected override async Task OnInitializedAsync()
    {
        await ReloadAsync();
    }

    protected async Task HandleValidSubmitAsync()
    {
        if (!TryBuildInput(out var input))
        {
            SetStatus("時刻は HH:mm 形式で入力してください。", isError: true);
            return;
        }

        if (IsEditMode && EditingRecordId.HasValue)
        {
            var result = await UpdateService.UpdateAsync(EditingRecordId.Value, EditingConcurrencyToken, input);
            await HandleResultAsync(result, successMessage: "更新しました。");
            return;
        }

        var createResult = await CreateService.CreateAsync(input);
        await HandleResultAsync(createResult, successMessage: "登録しました。");
    }

    protected async Task ReloadAsync()
    {
        Items = (await ListQueryService.GetListAsync()).ToList();
    }

    protected void StartEdit(AttendanceListItem item)
    {
        EditingRecordId = item.AttendanceRecordId;
        EditingConcurrencyToken = item.ConcurrencyToken;
        InputModel = new AttendanceFormModel
        {
            EmployeeId = item.EmployeeId,
            WorkDate = item.WorkDate,
            StartTimeText = item.StartTime.ToString("HH:mm"),
            EndTimeText = item.EndTime.ToString("HH:mm"),
            BreakMinutes = item.BreakMinutes
        };

        SetStatus("編集モードに切り替えました。", isError: false);
    }

    protected void CancelEdit()
    {
        EditingRecordId = null;
        EditingConcurrencyToken = 0;
        InputModel = AttendanceFormModel.CreateDefault();
        StatusMessage = null;
    }

    protected async Task DeleteAsync(AttendanceListItem item)
    {
        var confirmed = await JSRuntime.InvokeAsync<bool>("confirm", "削除します。よろしいですか？");
        if (!confirmed)
        {
            return;
        }

        var result = await DeleteService.DeleteAsync(item.AttendanceRecordId, item.ConcurrencyToken);
        if (result.IsSuccess)
        {
            SetStatus(result.Message ?? "削除しました。", isError: false);
            await ReloadAsync();
            if (EditingRecordId == item.AttendanceRecordId)
            {
                CancelEdit();
            }

            return;
        }

        HandleErrorResult(result.Status, result.Errors, result.Message);
    }

    protected static string FormatWorkMinutes(int workMinutes)
    {
        var hours = workMinutes / 60;
        var minutes = workMinutes % 60;
        return $"{hours}:{minutes:00}";
    }

    private bool TryBuildInput(out AttendanceEditInput input)
    {
        input = new AttendanceEditInput
        {
            EmployeeId = InputModel.EmployeeId?.Trim() ?? string.Empty,
            WorkDate = InputModel.WorkDate,
            BreakMinutes = InputModel.BreakMinutes
        };

        if (!TimeOnly.TryParse(InputModel.StartTimeText, out var startTime) ||
            !TimeOnly.TryParse(InputModel.EndTimeText, out var endTime))
        {
            return false;
        }

        input.StartTime = startTime;
        input.EndTime = endTime;
        return true;
    }

    private async Task HandleResultAsync(
        AttendanceOperationResult<Domain.Entities.AttendanceRecord> result,
        string successMessage)
    {
        if (!result.IsSuccess)
        {
            HandleErrorResult(result.Status, result.Errors, result.Message);
            return;
        }

        SetStatus(result.Message ?? successMessage, isError: false);
        await ReloadAsync();
        CancelEdit();
    }

    private void HandleErrorResult(
        AttendanceOperationStatus status,
        IReadOnlyList<string> errors,
        string? message)
    {
        var text = status switch
        {
            AttendanceOperationStatus.Validation => string.Join(" ", errors),
            AttendanceOperationStatus.Conflict => message ?? "他の担当者が先に更新しました。再読込して再入力してください。",
            AttendanceOperationStatus.NotFound => message ?? "対象データが見つかりません。",
            _ => message ?? "処理に失敗しました。"
        };

        SetStatus(text, isError: true);
    }

    private void SetStatus(string message, bool isError)
    {
        StatusMessage = message;
        StatusAlertCss = isError ? "alert-danger" : "alert-success";
    }

    protected sealed class AttendanceFormModel
    {
        [Required]
        public string EmployeeId { get; set; } = string.Empty;

        [Required]
        public DateOnly WorkDate { get; set; } = DateOnly.FromDateTime(DateTime.Today);

        [Required]
        public string StartTimeText { get; set; } = "09:00";

        [Required]
        public string EndTimeText { get; set; } = "18:00";

        [Range(0, 24 * 60)]
        public int BreakMinutes { get; set; } = 60;

        public static AttendanceFormModel CreateDefault() => new();
    }
}
