using CCP.RoleAccessScanner.Interfaces;

namespace WebApplication2.Models
{
    public partial class RoleAccessLog : IRoleAccessRecord
    {
        public int Id { get; set; }
        public string ProjectId { get; set; }
        public string ProjectName { get; set; }
        public string Role { get; set; }
        public string Controller { get; set; }
        public string Action { get; set; }
        public string Type { get; set; } // "Page" หรือ "Action"
        public string Remark { get; set; }
        public DateTime LoggedAt { get; set; }
    }

}
