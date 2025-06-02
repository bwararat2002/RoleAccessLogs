using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using CCP.RoleAccessScanner.Services;

namespace CCP.RoleAccessScanner.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddRoleAccessScanner<TDbContext>(this IServiceCollection services, string projectId, string projectName)
        where TDbContext : DbContext
    {
        services.AddSingleton(new RoleAccessScannerConfig { ProjectId = projectId, ProjectName = projectName });
        services.AddHostedService<RoleAccessBackgroundService<TDbContext>>();
        return services;
    }
}

public class RoleAccessScannerConfig
{
    public required string ProjectId { get; set; }
    public required string ProjectName { get; set; }
}