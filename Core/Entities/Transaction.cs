using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Entities
{
    public class Transaction : BaseEntity
    {
        public Guid TransactionID { get; set; } = Guid.CreateVersion7();
        public required Guid PersonID { get; set; }    // Foreign Key
        public required Person Person { get; set; }  // Navigation Property
        public required Guid TransactionModeID { get; set; }    // Foreign Key
        public required TransactionMode TransactionMode { get; set; }  // Navigation Property
        public required Guid EquipmentID { get; set; }    // Foreign Key
        public required Equipment Equipment { get; set; }  // Navigation Property
        public Guid EquipmentStatusID { get; set; }      // Foreign Key
        public required EquipmentStatus Status { get; set; }  // Navigation Property
        public string? Description { get; set; }
    }
}
