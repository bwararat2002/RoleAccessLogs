using CCP.RoleAccessScanner.Interfaces;
using CCP.RoleAccessScanner.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;

namespace CCP.RoleAccessScanner.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddRoleAccessScanner<TDbContext, TModel>(
        this IServiceCollection services,
        IConfiguration configuration,
        string projectId,
        string projectName)
        where TDbContext : DbContext
        where TModel : class, IRoleAccessRecord, new()
    {
        var mappings = configuration.GetSection("AuthorizationPolicies")
            .Get<Dictionary<string, List<string>>>();

        services.AddSingleton(new RoleAccessScannerConfig
        {
            ProjectId = projectId,
            ProjectName = projectName,
            RoleMappings = mappings
        });

        services.AddHostedService<RoleAccessBackgroundService<TDbContext, TModel>>();
        return services;
    }

}

public class RoleAccessScannerConfig
{
    public required string ProjectId { get; set; }
    public required string ProjectName { get; set; }

    public Dictionary<string, List<string>>? RoleMappings { get; set; }
}