namespace IMS.BusinessModel.Dto
{
    public class LoseProfitReportDto
    {
        public PaymentDetailsDto PurchasePaymentDetails { get; set; } = new PaymentDetailsDto();
        public PaymentDetailsDto SalePaymentDetails { get; set; } = new PaymentDetailsDto();
    }
    public class PaymentDetailsDto
    {
        public int Count { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal PaidAmount { get; set; }
        public decimal DueAmount { get; set; }
    }
}
