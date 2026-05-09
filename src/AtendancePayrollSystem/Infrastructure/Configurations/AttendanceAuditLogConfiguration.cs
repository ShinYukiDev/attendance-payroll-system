using AtendancePayrollSystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AtendancePayrollSystem.Infrastructure.Configurations;

public sealed class AttendanceAuditLogConfiguration : IEntityTypeConfiguration<AttendanceAuditLog>
{
    public void Configure(EntityTypeBuilder<AttendanceAuditLog> builder)
    {
        builder.ToTable("AttendanceAuditLogs");
        builder.HasKey(x => x.AuditId);

        builder.Property(x => x.OccurredAtUtc).IsRequired();
        builder.Property(x => x.ActionType).HasMaxLength(32).IsRequired();
        builder.Property(x => x.ActorId).HasMaxLength(64);
        builder.Property(x => x.TargetEmployeeIdMasked).HasMaxLength(16).IsRequired();
        builder.Property(x => x.TargetWorkDate).IsRequired();
        builder.Property(x => x.ChangedFields).HasMaxLength(256).IsRequired();
        builder.Property(x => x.Result).HasMaxLength(16).IsRequired();
        builder.Property(x => x.ReasonCode).HasMaxLength(64);

        builder.HasIndex(x => x.OccurredAtUtc);
        builder.HasIndex(x => new { x.TargetRecordId, x.ActionType });
    }
}
