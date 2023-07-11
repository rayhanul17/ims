using System.Collections.Generic;

namespace IMS.Models.Entities
{
    public class Category : BaseEntity
    {
        public virtual string Name { get; set; }
        public virtual string Description { get; set; }
        public virtual IList<Product> Products { get; set; } = new List<Product>();
    }
}
