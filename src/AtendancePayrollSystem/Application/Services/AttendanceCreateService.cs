using AtendancePayrollSystem.Application.Contracts;
using AtendancePayrollSystem.Domain.Entities;
using AtendancePayrollSystem.Domain.Services;
using AtendancePayrollSystem.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace AtendancePayrollSystem.Application.Services;

public sealed class AttendanceCreateService(
    AttendanceDbContext dbContext,
    AttendanceValidationService validationService,
    AttendanceAuditService auditService)
{
    public async Task<AttendanceOperationResult<AttendanceRecord>> CreateAsync(
        AttendanceEditInput input,
        CancellationToken cancellationToken = default)
    {
        var errors = validationService.Validate(input);
        if (errors.Count > 0)
        {
            return AttendanceOperationResult<AttendanceRecord>.Validation(errors.ToArray());
        }

        var isDuplicate = await dbContext.AttendanceRecords
            .AnyAsync(x => x.EmployeeId == input.EmployeeId && x.WorkDate == input.WorkDate, cancellationToken);
        if (isDuplicate)
        {
            return AttendanceOperationResult<AttendanceRecord>.Validation("同一社員ID・勤務日の勤怠は既に登録されています。");
        }

        var now = DateTime.UtcNow;
        var record = new AttendanceRecord
        {
            AttendanceRecordId = Guid.NewGuid(),
            EmployeeId = input.EmployeeId,
            WorkDate = input.WorkDate,
            StartTime = input.StartTime,
            EndTime = input.EndTime,
            BreakMinutes = input.BreakMinutes,
            WorkMinutes = validationService.CalculateWorkMinutes(input),
            ConcurrencyToken = 1,
            CreatedAtUtc = now,
            UpdatedAtUtc = now,
            CreatedBy = "SYSTEM",
            UpdatedBy = "SYSTEM"
        };

        dbContext.AttendanceRecords.Add(record);

        try
        {
            await dbContext.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateException)
        {
            return AttendanceOperationResult<AttendanceRecord>.Validation("同一社員ID・勤務日の勤怠は既に登録されています。");
        }

        await auditService.WriteAsync(
            "CREATE",
            record.AttendanceRecordId,
            record.EmployeeId,
            record.WorkDate,
            "EmployeeId,WorkDate,StartTime,EndTime,BreakMinutes,WorkMinutes",
            "SUCCESS",
            cancellationToken: cancellationToken);

        return AttendanceOperationResult<AttendanceRecord>.Ok(record, "登録しました。");
    }
}
