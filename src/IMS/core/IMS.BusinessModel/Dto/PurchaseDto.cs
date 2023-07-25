using System;

namespace IMS.BusinessModel.Dto
{
    public class PurchaseDto
    {
        public virtual long Id { get; set; }
        public virtual long SupplierId { get; set; }
        public virtual long CreateBy { get; set; }
        public virtual DateTime PurchaseDate { get; set; }
        public virtual decimal GrandTotalPrice { get; set; }
    }
}
