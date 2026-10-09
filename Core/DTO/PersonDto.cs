using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Core.DTO
{
    public class PersonDto
    {
        public Guid? PersonID { get; set; }
        public  string? FirstName { get; set; }
        public  string? LastName { get; set; }
        [MaxLength(10)]
        public  string? NationaleCode { get; set; }
        [MaxLength(11)]
        public string? CellPhone { get; set; }
    }
}
