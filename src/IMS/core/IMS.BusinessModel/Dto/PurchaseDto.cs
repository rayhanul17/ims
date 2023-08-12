namespace IMS.BusinessModel.Dto
{
    public class PurchaseDto
    {
        public string Id { get; set; }
        public string SupplierName { get; set; }
        public string CreateBy { get; set; }
        public string PurchaseDate { get; set; }
        public string GrandTotalPrice { get; set; }
        public string IsPaid { get; set; }
        public string PaymentId { get; set; }
    }
}
