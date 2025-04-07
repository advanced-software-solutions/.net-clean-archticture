namespace CleanBase.Entities
{
    public class UserAccount : EntityRoot
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public Guid UserRoleId { get; set; }
        public UserRole? UserRole { get; set; }
    }
}
