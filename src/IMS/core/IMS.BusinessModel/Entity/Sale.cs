using System;
using System.Collections.Generic;

namespace IMS.BusinessModel.Entity
{
    public class Sale : IEntity<long>
    {
        public virtual long Id { get; set; }
        public virtual long CustomerId { get; set; }
        public virtual long CreateBy { get; set; }
        public virtual DateTime SaleDate { get; set; }
        public virtual decimal GrandTotalPrice { get; set; }     
        public virtual bool IsPaid { get; set; }
        public virtual long PaymentId { get; set; }
        public virtual IList<SaleDetails> SaleDetails { get; set; } = new List<SaleDetails>();
    }
}
