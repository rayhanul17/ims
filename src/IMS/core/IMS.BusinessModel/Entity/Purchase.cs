using System;
using System.Collections.Generic;

namespace IMS.BusinessModel.Entity
{
    public class Purchase : IEntity<long>
    {
        public virtual long Id { get; set; }
        public virtual long SupplierId { get; set; }
        public virtual long CreateBy { get; set; }
        public virtual DateTime PurchaseDate { get; set; }
        public virtual decimal GrandTotalPrice { get; set; }               
        public virtual IList<PurchaseDetails> PurchaseDetails { get; set; } = new List<PurchaseDetails>();
    }
}
