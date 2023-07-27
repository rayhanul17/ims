using System.Collections.Generic;

namespace IMS.BusinessModel.Entity
{
    public class Bank : BaseEntity<long>
    {        
        public virtual string Description { get; set; }   
        public virtual IList<Payment> Payment { get; set; } = new List<Payment>();
    }
}
