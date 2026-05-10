namespace AtendancePayrollSystem.Application.Contracts;

public sealed class OvertimeSearchCriteria
{
    public string EmployeeId { get; set; } = string.Empty;

    public string TargetMonth { get; set; } = string.Empty;
}
