
using Microsoft.EntityFrameworkCore;
using WebApplication2.Models;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<RoleAccessLog> RoleAccessLogs { get; set; } 
}
