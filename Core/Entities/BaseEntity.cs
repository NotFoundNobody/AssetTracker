using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Entities
{
    public abstract class BaseEntity
    {
        public DateTime DateCreate { get; set; } = DateTime.Now;

        public DateTime? DateModified { get; set; }

        public bool IsDeleted { get; set; } = false;
    }

}
