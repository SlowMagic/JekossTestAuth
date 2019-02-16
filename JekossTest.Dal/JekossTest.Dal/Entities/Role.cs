using System.Collections.Generic;

namespace JekossTest.Dal.Entities
{
    public class Role
    {
        public int Id { get; set; }
        public string RoleName { get; set; }
        public virtual List<User> Users { get; set; }
    }
}