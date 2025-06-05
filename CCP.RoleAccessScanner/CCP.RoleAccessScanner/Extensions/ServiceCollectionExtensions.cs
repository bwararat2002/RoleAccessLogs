using CCP.RoleAccessScanner.Interfaces;
using CCP.RoleAccessScanner.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace CCP.RoleAccessScanner.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddRoleAccessScanner<TDbContext, TModel>(
        this IServiceCollection services,
        string projectId,
        string projectName)
        where TDbContext : DbContext
        where TModel : class, IRoleAccessRecord, new()
    {
        services.AddSingleton(new RoleAccessScannerConfig
        {
            ProjectId = projectId,
            ProjectName = projectName
        });

        services.AddHostedService<RoleAccessBackgroundService<TDbContext, TModel>>();

        return services;
    }

}

public class RoleAccessScannerConfig
{
    public required string ProjectId { get; set; }
    public required string ProjectName { get; set; }
}