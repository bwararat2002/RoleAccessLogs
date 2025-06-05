#nullable enable
using CCP.RoleAccessScanner.Extensions;
using CCP.RoleAccessScanner.Interfaces;
using CCP.RoleAccessScanner.Internal;
using CCP.RoleAccessScanner.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace CCP.RoleAccessScanner.Services;

// ?? เพิ่ม TModel เป็น generic
public class RoleAccessBackgroundService<TDbContext, TModel> : BackgroundService
    where TDbContext : DbContext
    where TModel : class, IRoleAccessRecord, new()
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

        // ?? เรียกใช้แบบ generic ที่รองรับ TModel
        Internal.RoleAccessScanner.ScanAndLogRoles<TDbContext, TModel>(
            db, _env, _config.ProjectId, _config.ProjectName);

        return Task.CompletedTask;
    }
}
