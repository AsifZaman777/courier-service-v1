namespace Courier_Service_V1.Models
{
    public class Admin
    {
        public string Id { get; set; } = "AD-" + Guid.NewGuid().ToString().Substring(0, 4);
        public string Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
    }
}
