using Microsoft.EntityFrameworkCore;
using CCP.RoleAccessScanner.Models; // ✅ ใช้ของ NuGet

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<RoleAccessLog> RoleAccessLogs { get; set; } // ✅ ใช้จาก NuGet
}
