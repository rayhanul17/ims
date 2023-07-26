using System;

namespace IMS.BusinessModel.Dto
{
    public class SaleDto
    {
        public virtual long Id { get; set; }
        public virtual long CustomerId { get; set; }
        public virtual long CreateBy { get; set; }
        public virtual DateTime SaleDate { get; set; }
        public virtual decimal GrandTotalPrice { get; set; }
    }
}
