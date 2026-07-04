using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Entities
{
    public class Role : BaseEntity
    {
        public Guid RoleID { get; set; } = Guid.CreateVersion7();

        public required string Name { get; set; }

        public string? Description { get; set; }

        public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();

        public ICollection<RolePermission> RolePermissions { get; set; } = new List<RolePermission>();
    }

}
