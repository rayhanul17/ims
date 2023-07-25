namespace IMS.BusinessModel.ViewModel
{
    public class PurchaseDetailsModel
    {  
        public bool IsDeleted { get; set; }
        public long ProductId { get; set; }
        public decimal Quantity { get; set; }
        public int UnitPrice { get; set; }
        public decimal Total { get; set; }
    }
}
