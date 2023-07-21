using System.Collections.Generic;

namespace IMS.BusinessModel.Entity
{
    public class Category : BaseEntity<long>
    {        
        public virtual string Description { get; set; }
        public virtual IList<Product> Products { get; set; } = new List<Product>();
    }
}
