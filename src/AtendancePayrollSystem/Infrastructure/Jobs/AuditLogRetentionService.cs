using AtendancePayrollSystem.Application.Services;
using Microsoft.Extensions.Options;

namespace AtendancePayrollSystem.Infrastructure.Jobs;

public sealed class AuditLogRetentionService(
    IServiceProvider serviceProvider,
    IOptions<AttendanceAuditOptions> options,
    ILogger<AuditLogRetentionService> logger) : BackgroundService
{
    private readonly AttendanceAuditOptions _options = options.Value;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = serviceProvider.CreateScope();
                var auditService = scope.ServiceProvider.GetRequiredService<AttendanceAuditService>();
                var deletedCount = await auditService.CleanupExpiredLogsAsync(_options.RetentionDays, stoppingToken);
                logger.LogInformation("Expired audit logs cleanup completed. DeletedCount={DeletedCount}", deletedCount);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to cleanup expired audit logs.");
            }

            await Task.Delay(TimeSpan.FromDays(1), stoppingToken);
        }
    }
}
