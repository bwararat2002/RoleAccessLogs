#nullable enable
using CCP.RoleAccessScanner.Interfaces;
using System;
namespace CCP.RoleAccessScanner.Models;

public class RoleAccessLog : IRoleAccessRecord
{
    public int Id { get; set; }
    public string ProjectId { get; set; }
    public string ProjectName { get; set; }
    public string Role { get; set; }
    public string Controller { get; set; }
    public string Action { get; set; }
    public string Type { get; set; }
    public DateTime LoggedAt { get; set; }
}