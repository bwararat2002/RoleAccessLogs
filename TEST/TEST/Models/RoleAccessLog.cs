namespace WebApplication2.Models
{
    public class RoleAccessLog
    {
        public int Id { get; set; }
        public string ProjectId { get; set; }
        public string Role { get; set; }
        public string Controller { get; set; }
        public string Action { get; set; }
        public string Type { get; set; } // "Page" หรือ "Action"
        public DateTime LoggedAt { get; set; }
    }

}
