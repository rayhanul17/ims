using System.Collections.Generic;

namespace IMS.BusinessModel.Entity
{
    public class Bank : BaseEntity<long>
    {        
        public virtual string Description { get; set; }   
        public virtual IList<PaymentDetails> PaymentDetails { get; set; } = new List<PaymentDetails>();
    }
}
