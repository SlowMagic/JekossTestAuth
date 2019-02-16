namespace JekossTest.Dal.Entities
{
    public class User 
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Phone { get; set; }
        
        public int? RoleId { get; set; }
        public virtual Role Role { get; set; }
        public int AccountRefreshTokenId { get; set; }
        public virtual AccountRefreshToken AccountRefreshToken { get; set; }
    }
}