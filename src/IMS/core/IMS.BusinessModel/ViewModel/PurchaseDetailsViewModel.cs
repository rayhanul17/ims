namespace IMS.BusinessModel.ViewModel
{
    public class PurchaseDetailsViewModel
    {
        public bool IsDeleted { get; set; }
        public long ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal Total { get; set; }
    }
}
