using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CCP.RoleAccessScanner.Interfaces
{
    public interface IRoleAccessRecord
    {
        string ProjectId { get; set; }
        string ProjectName { get; set; }
        string Controller { get; set; }
        string Action { get; set; }
        string Role { get; set; }
        string Type { get; set; }
        DateTime LoggedAt { get; set; }
    }
}
