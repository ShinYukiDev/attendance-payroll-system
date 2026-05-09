using AtendancePayrollSystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AtendancePayrollSystem.Infrastructure.Configurations;

public sealed class AttendanceRecordConfiguration : IEntityTypeConfiguration<AttendanceRecord>
{
    public void Configure(EntityTypeBuilder<AttendanceRecord> builder)
    {
        builder.ToTable("AttendanceRecords");
        builder.HasKey(x => x.AttendanceRecordId);

        builder.Property(x => x.EmployeeId)
            .HasMaxLength(7)
            .IsRequired();

        builder.Property(x => x.WorkDate).IsRequired();
        builder.Property(x => x.StartTime).IsRequired();
        builder.Property(x => x.EndTime).IsRequired();
        builder.Property(x => x.BreakMinutes).IsRequired();
        builder.Property(x => x.WorkMinutes).IsRequired();

        builder.Property(x => x.ConcurrencyToken)
            .IsConcurrencyToken()
            .IsRequired();

        builder.Property(x => x.CreatedAtUtc).IsRequired();
        builder.Property(x => x.UpdatedAtUtc).IsRequired();
        builder.Property(x => x.CreatedBy).HasMaxLength(64);
        builder.Property(x => x.UpdatedBy).HasMaxLength(64);

        builder.HasIndex(x => new { x.EmployeeId, x.WorkDate })
            .IsUnique();
    }
}
