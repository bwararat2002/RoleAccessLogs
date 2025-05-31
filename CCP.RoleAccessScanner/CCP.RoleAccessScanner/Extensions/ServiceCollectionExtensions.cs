using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using CCP.RoleAccessScanner.Services;

namespace CCP.RoleAccessScanner.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddRoleAccessScanner<TDbContext>(this IServiceCollection services, string projectId)
        where TDbContext : DbContext
    {
        services.AddSingleton(new RoleAccessScannerConfig { ProjectId = projectId });
        services.AddHostedService<RoleAccessBackgroundService<TDbContext>>();
        return services;
    }
}

public class RoleAccessScannerConfig
{
    public string ProjectId { get; set; }
}