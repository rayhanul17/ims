using System;
using System.Collections.Generic;
using System.Text;

namespace IMS.Models.Entities
{
    public class Product : BaseEntity
    {
        public virtual string Name { get; set; }
        public virtual string Description { get; set; }
        public virtual long CategoryId { get; set; }
        public virtual Category Category { get; set; }
    }
}
