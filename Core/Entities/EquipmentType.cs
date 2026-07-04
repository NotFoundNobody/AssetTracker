using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Entities
{
    public class EquipmentType
    {
        public Guid EquipmentTypeID {  get; set; }= Guid.CreateVersion7();
        public required string Name { get; set; }
        public  string? Description { get; set; }
        public required DateTime DateCreate { get; set; } = DateTime.Now;
        public DateTime? DateModified { get; set; }
        public required bool IsDeleted=false;

    }
}
