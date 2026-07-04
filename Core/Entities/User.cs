using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Entities
{
    public class User : BaseEntity
    {
        public Guid UserID { get; set; } = Guid.CreateVersion7();

        public required string Username { get; set; }

        public required string PasswordHash { get; set; }


        public bool IsActive { get; set; } = true;

        public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
        public required Guid PersonID { get; set; }     // Foreign Key
        public required Person Person { get; set; }       // Navigation Property
    }

}
