using System;
using System.Collections.Generic;

namespace IMS.BusinessModel.Entity
{
    public class Payment : IEntity<long>
    {
        public virtual long Id { get; set; }
        public virtual long OperationId { get; set; }
        public virtual int OperationType { get; set; }
        public virtual decimal TotalAmount { get; set; }
        public virtual decimal PaidAmount { get; set; }  
        public virtual IList<PaymentDetails> PaymentDetails { get; set; } = new List<PaymentDetails>();
    }
}
