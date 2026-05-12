using AtendancePayrollSystem.Application.Contracts;
using AtendancePayrollSystem.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace AtendancePayrollSystem.Application.Queries;

public sealed class AttendanceListQueryService(AttendanceDbContext dbContext)
{
    public async Task<IReadOnlyList<AttendanceListItem>> GetListAsync(CancellationToken cancellationToken = default)
    {
        return await dbContext.AttendanceRecords
            .AsNoTracking()
            .OrderBy(x => x.EmployeeId)
            .ThenBy(x => x.WorkDate)
            .Select(x => new AttendanceListItem
            {
                AttendanceRecordId = x.AttendanceRecordId,
                EmployeeId = x.EmployeeId,
                WorkDate = x.WorkDate,
                StartTime = x.StartTime,
                EndTime = x.EndTime,
                BreakMinutes = x.BreakMinutes,
                WorkMinutes = x.WorkMinutes,
                ConcurrencyToken = x.ConcurrencyToken
            })
            .ToListAsync(cancellationToken);
    }
}
