namespace IMS.BusinessModel.ViewModel
{
    public class ProductBuyingSellingQueryRequestViewModel
    {
        public long? CategoryId { get; set; }
        public long? BrandId { get; set; }
        public long? ProductId { get; set; }
        public long? SupplierId { get; set; }
        public long? CustomerId { get; set; }
        public string DateRange { get; set; }
    }
}
