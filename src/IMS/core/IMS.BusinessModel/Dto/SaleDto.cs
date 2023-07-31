using System;

namespace IMS.BusinessModel.Dto
{
    public class SaleDto
    {
        public long Id { get; set; }
        public long CustomerId { get; set; }
        public long CreateBy { get; set; }
        public DateTime SaleDate { get; set; }
        public decimal GrandTotalPrice { get; set; }
        public bool IsPaid { get; set; }
        public long PaymentId { get; set; }
    }
}
