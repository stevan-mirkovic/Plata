namespace Plata.Models.Entities
{
    public partial class UserAccount
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public Company Company { get; set; }
    }
}