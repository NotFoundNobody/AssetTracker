using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Entities
{
    public class EquipmentStatus  :BaseEntity
    {
        public required Guid EquipmentStatusID { get; set; } = Guid.CreateVersion7();
        public required string Name { get; set; }
        public string? Description { get; set; }
    }
}
