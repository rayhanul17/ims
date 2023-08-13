namespace IMS.BusinessModel.Dto
{
    public class SaleDto
    {
        public string Id { get; set; }
        public string CustomerName { get; set; }
        public string CreateBy { get; set; }
        public string SaleDate { get; set; }
        public string GrandTotalPrice { get; set; }
        public string IsPaid { get; set; }
        public string PaymentId { get; set; }
        public string Rank { get; set; }
    }
}
