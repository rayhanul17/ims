using System;
using System.Collections.Generic;

namespace IMS.BusinessModel.Entity
{
    public class Purchase : BaseEntity<long>
    {
        public virtual long SupplierId { get; set; }
        public virtual DateTime PurchaseDate { get; set; }
        public virtual decimal GrandTotalPrice { get; set; }
        public virtual bool IsPaid { get; set; }
        public virtual long PaymentId { get; set; }
        public virtual IList<PurchaseDetails> PurchaseDetails { get; set; } = new List<PurchaseDetails>();
    }
}
