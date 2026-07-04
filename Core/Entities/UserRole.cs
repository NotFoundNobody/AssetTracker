using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Entities
{
    public class UserRole
    {
        public Guid UserID { get; set; }

        public Guid RoleID { get; set; }

        public User User { get; set; } = null!;

        public Role Role { get; set; } = null!;
    }

}
