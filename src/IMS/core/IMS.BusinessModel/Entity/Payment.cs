using System.Collections.Generic;

namespace IMS.BusinessModel.Entity
{
    public class Payment : BaseEntity<long>
    {
        public virtual long? PurchaseId { get; set; }
        public virtual long? SaleId { get; set; }
        public virtual string VoucherId { get; set; }
        public virtual int OperationType { get; set; }
        public virtual decimal TotalAmount { get; set; }
        public virtual decimal PaidAmount { get; set; }
        public virtual IList<PaymentDetails> PaymentDetails { get; set; } = new List<PaymentDetails>();
    }
}
