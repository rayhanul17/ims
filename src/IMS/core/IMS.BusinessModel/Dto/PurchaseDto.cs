using System;

namespace IMS.BusinessModel.Dto
{
    public class PurchaseDto
    {
        public long Id { get; set; }
        public long SupplierId { get; set; }
        public long CreateBy { get; set; }
        public DateTime PurchaseDate { get; set; }
        public decimal GrandTotalPrice { get; set; }
        public bool IsPaid { get; set; }
    }
}
