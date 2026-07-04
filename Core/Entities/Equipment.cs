using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Entities
{
    public class Equipment:BaseEntity
    {
        private long _assetNumber;
        public required Guid EquipmentID { get; set; }=Guid.CreateVersion7();
        public required long EquipmentAssetNumber
        {
            get => _assetNumber == 0 ? GenerateAssetNumber(8) : _assetNumber;
            set => _assetNumber = value;
        }
        public required string Name { get; set;  }
        public string? Description { get; set; }
        public required Guid EquipmentTypeID {  get; set; }    // Foreign Key
        public required EquipmentType Type { get; set; }  // Navigation Property

        public Guid EquipmentStatusID { get; set; }      // Foreign Key
        public required EquipmentStatus Status { get; set; }  // Navigation Property
        private static long GenerateAssetNumber(int digits)
        {
            var random = new Random();
            long min = (long)Math.Pow(10, digits - 1);
            long max = (long)Math.Pow(10, digits) - 1;
            return random.NextInt64(min, max);
        }
    }

}
