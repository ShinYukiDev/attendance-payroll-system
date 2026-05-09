using AtendancePayrollSystem.Application.Contracts;
using AtendancePayrollSystem.Domain.Entities;
using AtendancePayrollSystem.Domain.Services;
using AtendancePayrollSystem.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace AtendancePayrollSystem.Application.Services;

public sealed class AttendanceUpdateService(
    AttendanceDbContext dbContext,
    AttendanceValidationService validationService,
    AttendanceAuditService auditService)
{
    public async Task<AttendanceOperationResult<AttendanceRecord>> UpdateAsync(
        Guid recordId,
        int concurrencyToken,
        AttendanceEditInput input,
        CancellationToken cancellationToken = default)
    {
        var errors = validationService.Validate(input);
        if (errors.Count > 0)
        {
            return AttendanceOperationResult<AttendanceRecord>.Validation(errors.ToArray());
        }

        var record = await dbContext.AttendanceRecords
            .FirstOrDefaultAsync(x => x.AttendanceRecordId == recordId, cancellationToken);
        if (record is null)
        {
            return AttendanceOperationResult<AttendanceRecord>.NotFound("対象データが見つかりません。");
        }

        if (record.ConcurrencyToken != concurrencyToken)
        {
            await auditService.WriteConflictAsync(
                record.AttendanceRecordId,
                record.EmployeeId,
                record.WorkDate,
                "TOKEN_MISMATCH",
                cancellationToken);
            return AttendanceOperationResult<AttendanceRecord>.Conflict("他の担当者が先に更新しました。再読込して再入力してください。");
        }

        var duplicateExists = await dbContext.AttendanceRecords
            .AnyAsync(x => x.AttendanceRecordId != recordId && x.EmployeeId == input.EmployeeId && x.WorkDate == input.WorkDate, cancellationToken);
        if (duplicateExists)
        {
            return AttendanceOperationResult<AttendanceRecord>.Validation("同一社員ID・勤務日の勤怠は既に登録されています。");
        }

        record.EmployeeId = input.EmployeeId;
        record.WorkDate = input.WorkDate;
        record.StartTime = input.StartTime;
        record.EndTime = input.EndTime;
        record.BreakMinutes = input.BreakMinutes;
        record.WorkMinutes = validationService.CalculateWorkMinutes(input);
        record.UpdatedAtUtc = DateTime.UtcNow;
        record.UpdatedBy = "SYSTEM";
        record.ConcurrencyToken += 1;

        try
        {
            await dbContext.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateConcurrencyException)
        {
            await auditService.WriteConflictAsync(
                recordId,
                input.EmployeeId,
                input.WorkDate,
                "DB_CONCURRENCY_EXCEPTION",
                cancellationToken);
            return AttendanceOperationResult<AttendanceRecord>.Conflict("他の担当者が先に更新しました。再読込して再入力してください。");
        }
        catch (DbUpdateException)
        {
            return AttendanceOperationResult<AttendanceRecord>.Validation("同一社員ID・勤務日の勤怠は既に登録されています。");
        }

        await auditService.WriteAsync(
            "UPDATE",
            record.AttendanceRecordId,
            record.EmployeeId,
            record.WorkDate,
            "EmployeeId,WorkDate,StartTime,EndTime,BreakMinutes,WorkMinutes",
            "SUCCESS",
            cancellationToken: cancellationToken);

        return AttendanceOperationResult<AttendanceRecord>.Ok(record, "更新しました。");
    }
}
