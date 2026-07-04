using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Core.Entities
{
    public class Person :BaseEntity
    {
        public Guid PersonID { get; set; } = Guid.CreateVersion7();
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        [MaxLength(10)]
        public required string NationaleCode { get; set; }
        [MaxLength(11)]
        public required string CellPhone { get; set; }

    }
}
