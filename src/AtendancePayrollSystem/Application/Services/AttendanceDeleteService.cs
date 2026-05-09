using AtendancePayrollSystem.Application.Contracts;
using AtendancePayrollSystem.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace AtendancePayrollSystem.Application.Services;

public sealed class AttendanceDeleteService(
    AttendanceDbContext dbContext,
    AttendanceAuditService auditService)
{
    public async Task<AttendanceOperationResult> DeleteAsync(
        Guid recordId,
        int concurrencyToken,
        CancellationToken cancellationToken = default)
    {
        var record = await dbContext.AttendanceRecords
            .FirstOrDefaultAsync(x => x.AttendanceRecordId == recordId, cancellationToken);
        if (record is null)
        {
            return AttendanceOperationResult.NotFound("対象データが見つかりません。");
        }

        if (record.ConcurrencyToken != concurrencyToken)
        {
            await auditService.WriteConflictAsync(
                record.AttendanceRecordId,
                record.EmployeeId,
                record.WorkDate,
                "TOKEN_MISMATCH",
                cancellationToken);
            return AttendanceOperationResult.Conflict("他の担当者が先に更新しました。再読込して再入力してください。");
        }

        var employeeId = record.EmployeeId;
        var workDate = record.WorkDate;

        dbContext.AttendanceRecords.Remove(record);
        await dbContext.SaveChangesAsync(cancellationToken);

        await auditService.WriteAsync(
            "DELETE",
            recordId,
            employeeId,
            workDate,
            string.Empty,
            "SUCCESS",
            cancellationToken: cancellationToken);

        return AttendanceOperationResult.Ok("削除しました。");
    }
}
