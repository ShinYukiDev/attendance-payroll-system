using AtendancePayrollSystem.Components;
using AtendancePayrollSystem.Application.Queries;
using AtendancePayrollSystem.Application.Services;
using AtendancePayrollSystem.Domain.Services;
using AtendancePayrollSystem.Infrastructure;
using AtendancePayrollSystem.Infrastructure.Jobs;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.Configure<AttendanceAuditOptions>(
    builder.Configuration.GetSection("AttendanceAudit"));

builder.Services.AddDbContext<AttendanceDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("AttendanceDb")));

builder.Services.AddScoped<AttendanceValidationService>();
builder.Services.AddScoped<AttendanceAuditService>();
builder.Services.AddScoped<AttendanceCreateService>();
builder.Services.AddScoped<AttendanceUpdateService>();
builder.Services.AddScoped<AttendanceDeleteService>();
builder.Services.AddScoped<AttendanceListQueryService>();
builder.Services.AddScoped<OvertimeCalculationService>();
builder.Services.AddScoped<OvertimeReviewValidationService>();
builder.Services.AddScoped<OvertimeReviewQueryService>();
builder.Services.AddHostedService<AuditLogRetentionService>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AttendanceDbContext>();
    dbContext.Database.EnsureCreated();
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
app.UseHttpsRedirection();

app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
