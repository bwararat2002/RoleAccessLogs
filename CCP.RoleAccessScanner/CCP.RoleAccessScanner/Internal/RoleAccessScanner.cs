#nullable enable
using System.Threading.Tasks;
using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using Microsoft.EntityFrameworkCore;
using CCP.RoleAccessScanner.Models;
using System.Reflection;
using System.Linq;
using System.Collections.Generic;
using NonActionAttribute = Microsoft.AspNetCore.Mvc.NonActionAttribute;
using AuthorizeAttribute = Microsoft.AspNetCore.Authorization.AuthorizeAttribute;
using HttpPostAttribute = Microsoft.AspNetCore.Mvc.HttpPostAttribute;
using Microsoft.AspNetCore.Hosting;
using System.IO;

namespace CCP.RoleAccessScanner.Internal;

public static class RoleAccessScanner
{
    public static void ScanAndLogRoles<TDbContext>(TDbContext context, IWebHostEnvironment env, string projectId)
        where TDbContext : DbContext
    {
        var db = context.Set<RoleAccessLog>();
        var newAccessList = new List<RoleAccessLog>();

        var controllers = Assembly.GetEntryAssembly().GetTypes()
            .Where(t => typeof(Controller).IsAssignableFrom(t) && !t.IsAbstract);

        foreach (var controller in controllers)
        {
            var controllerName = controller.Name.Replace("Controller", "");
            var controllerAuthorize = controller.GetCustomAttribute<AuthorizeAttribute>();
            var controllerRoles = GetRolesFromAuthorize(controllerAuthorize);

            var actions = controller.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly)
                .Where(m =>
                    !m.IsDefined(typeof(NonActionAttribute)) &&
                    (typeof(IActionResult).IsAssignableFrom(m.ReturnType) ||
                     typeof(Task<IActionResult>).IsAssignableFrom(m.ReturnType)));

            foreach (var action in actions)
            {
                var actionAuthorize = action.GetCustomAttribute<AuthorizeAttribute>();
                var actionRoles = GetRolesFromAuthorize(actionAuthorize) ?? controllerRoles ?? new[] { "*" };

                string type = ViewExists(controllerName, action.Name, env) ? "Page" :
                              action.IsDefined(typeof(HttpPostAttribute), false) ||
                              action.Name.ToLower().Contains("save") ||
                              action.Name.ToLower().Contains("delete") ||
                              action.Name.ToLower().Contains("update") ? "Action" : "Unknown";

                foreach (var role in actionRoles)
                {
                    newAccessList.Add(new RoleAccessLog
                    {
                        ProjectId = projectId,
                        Controller = controllerName,
                        Action = action.Name,
                        Role = role,
                        Type = type,
                        LoggedAt = DateTime.Now
                    });
                }
            }
        }

        var existing = db.Where(x => x.ProjectId == projectId).ToList();

        var toAdd = newAccessList.Where(n => !existing.Any(e =>
            e.Controller == n.Controller &&
            e.Action == n.Action &&
            e.Role == n.Role)).ToList();

        var toUpdate = existing.Where(e =>
            newAccessList.Any(n =>
                n.Controller == e.Controller &&
                n.Action == e.Action &&
                n.Role == e.Role &&
                n.Type != e.Type)).ToList();

        var toRemove = existing.Where(e =>
            !newAccessList.Any(n =>
                n.Controller == e.Controller &&
                n.Action == e.Action &&
                n.Role == e.Role)).ToList();

        if (toRemove.Any()) db.RemoveRange(toRemove);
        if (toAdd.Any()) db.AddRange(toAdd);
        foreach (var item in toUpdate)
        {
            var match = newAccessList.First(n =>
                n.Controller == item.Controller &&
                n.Action == item.Action &&
                n.Role == item.Role);
            item.Type = match.Type;
            item.LoggedAt = DateTime.Now;
        }

        context.SaveChanges();
    }

    private static string[] GetRolesFromAuthorize(AuthorizeAttribute? attr)
    {
        if (attr == null) return null;
        if (!string.IsNullOrEmpty(attr.Roles)) return attr.Roles.Split(',').Select(r => r.Trim()).ToArray();
        if (!string.IsNullOrEmpty(attr.Policy)) return new[] { attr.Policy };
        return null;
    }

    private static bool ViewExists(string controllerName, string actionName, IWebHostEnvironment env)
    {
        var viewPath = Path.Combine(env.ContentRootPath, "Views", controllerName, $"{actionName}.cshtml");
        return File.Exists(viewPath);
    }
}