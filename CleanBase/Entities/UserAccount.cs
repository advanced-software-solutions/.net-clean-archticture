namespace CleanBase.Entities
{
    public class UserAccount : EntityRoot
    {
        public string Email { get; set; }
        public Guid UserRoleId { get; set; }
        public UserRole? UserRole { get; set; }
        private string password;
        public string Password { set { password = value; } }
    }
}
