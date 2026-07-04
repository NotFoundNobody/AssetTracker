using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Entities
{
    public class Permission : BaseEntity
    {
        public Guid PermissionID { get; set; } = Guid.CreateVersion7();

        public required string Name { get; set; }

        public string? Description { get; set; }

        public ICollection<RolePermission> RolePermissions { get; set; } = new List<RolePermission>();
    }

}
