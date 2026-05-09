using AtendancePayrollSystem.Infrastructure;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace AtendancePayrollSystem.Tests.Infrastructure;

internal static class TestDbContextFactory
{
    public static (AttendanceDbContext DbContext, SqliteConnection Connection) CreateSqliteContext()
    {
        var connection = new SqliteConnection("Data Source=:memory:");
        connection.Open();

        var options = new DbContextOptionsBuilder<AttendanceDbContext>()
            .UseSqlite(connection)
            .Options;

        var dbContext = new AttendanceDbContext(options);
        dbContext.Database.EnsureCreated();

        return (dbContext, connection);
    }
}
