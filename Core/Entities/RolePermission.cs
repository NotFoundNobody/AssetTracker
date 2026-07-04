using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Entities
{
    public class RolePermission
    {
        public Guid RoleID { get; set; }

        public Guid PermissionID { get; set; }

        public Role Role { get; set; } = null!;

        public Permission Permission { get; set; } = null!;
    }

}
