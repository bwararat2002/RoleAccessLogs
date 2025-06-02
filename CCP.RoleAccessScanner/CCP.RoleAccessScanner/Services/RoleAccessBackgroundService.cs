#nullable enable
using CCP.RoleAccessScanner.Extensions;
using CCP.RoleAccessScanner.Internal;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace CCP.RoleAccessScanner.Services;

public class RoleAccessBackgroundService<TDbContext> : BackgroundService where TDbContext : DbContext
{
    private readonly IServiceProvider _provider;
    private readonly IWebHostEnvironment _env;
    private readonly RoleAccessScannerConfig _config;

    public RoleAccessBackgroundService(IServiceProvider provider, IWebHostEnvironment env, RoleAccessScannerConfig config)
    {
        _provider = provider;
        _env = env;
        _config = config;
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var scope = _provider.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<TDbContext>();

        Internal.RoleAccessScanner.ScanAndLogRoles(db, _env, _config.ProjectId, _config.ProjectName);
        return Task.CompletedTask;
    }
}