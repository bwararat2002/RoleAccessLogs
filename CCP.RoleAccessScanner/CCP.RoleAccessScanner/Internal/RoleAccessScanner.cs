#nullable enable
using CCP.RoleAccessScanner.Attributes;
using CCP.RoleAccessScanner.Interfaces;
using CCP.RoleAccessScanner.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using AuthorizeAttribute = Microsoft.AspNetCore.Authorization.AuthorizeAttribute;
using HttpPostAttribute = Microsoft.AspNetCore.Mvc.HttpPostAttribute;
using NonActionAttribute = Microsoft.AspNetCore.Mvc.NonActionAttribute;

namespace CCP.RoleAccessScanner.Internal;

public static class RoleAccessScanner
{
    public static void ScanAndLogRoles<TDbContext, TModel>(TDbContext context, IWebHostEnvironment env, string projectId, string projectName, Dictionary<string, List<string>>? roleMap)
        where TDbContext : DbContext
        where TModel : class, IRoleAccessRecord, new()
    {
        var db = context.Set<TModel>();
        var newAccessList = new List<TModel>();

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
                var rawRoles = GetRolesFromAuthorize(actionAuthorize) ?? controllerRoles ?? new[] { "*" };
                var baseRoles = GetRolesFromAuthorize(actionAuthorize) ?? controllerRoles ?? new[] { "*" };
                var actionRoles = ExpandRoles(baseRoles, roleMap);


                var remarkPageAttr = action.GetCustomAttribute<RemarkPageAttribute>();
                var remarkPage = remarkPageAttr?.Title;

                var isPostAction = action.IsDefined(typeof(HttpPostAttribute), false)
                    || action.IsDefined(typeof(HttpPutAttribute), false)
                    || action.IsDefined(typeof(HttpDeleteAttribute), false)
                    || action.Name.ToLower().Contains("btn")
                    || action.Name.ToLower().Contains("button");

                var hasView = ViewExists(controllerName, action.Name, env);

                // เช็คว่ามี method GET ที่ชื่อเดียวกัน
                var hasGetSameName = controller.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly)
                    .Any(m => m.Name == action.Name &&
                              m != action &&
                              !m.IsDefined(typeof(NonActionAttribute)) &&
                              !m.IsDefined(typeof(HttpPostAttribute)) &&
                              !m.IsDefined(typeof(HttpPutAttribute)) &&
                              !m.IsDefined(typeof(HttpDeleteAttribute)));

                // [?] ถ้าเป็น POST และมี View และมี GET ชื่อเดียวกัน -> ให้ข้าม
                if (isPostAction && hasView && hasGetSameName)
                {
                    continue;
                }

                string type;
                if (hasView)
                {
                    type = "Page"; // มี View ถือเป็น Page เสมอ ไม่ว่าจะเป็น GET หรือ POST
                }
                else if (isPostAction)
                {
                    type = "Button";
                }
                else
                {
                    type = "Event";
                }


                foreach (var role in actionRoles)
                    {
                        newAccessList.Add(new TModel
                        {
                            ProjectId = projectId,
                            ProjectName = projectName,
                            Controller = controllerName,
                            Action = action.Name,
                            Role = role,
                            Type = type,
                            LoggedAt = DateTime.Now,
                            Remark = remarkPage ?? string.Empty,
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
                (n.Type != e.Type || n.Remark != e.Remark))).ToList();

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
            item.Remark = match.Remark;
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

    private static string[] ExpandRoles(string[] roles, Dictionary<string, List<string>>? mappings)
    {
        if (mappings == null) return roles;

        return roles
            .SelectMany(role =>
                mappings.ContainsKey(role) ? mappings[role] : new List<string> { role })
            .Distinct()
            .ToArray();
    }

}