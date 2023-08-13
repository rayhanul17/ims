namespace IMS.BusinessModel.Dto
{
    public class ProductDto : BaseDto
    {
        public string Description { get; set; }
        public string Image { get; set; }
        public string BuyingPrice { get; set; }
        public string SellingPrice { get; set; }
        public string DiscountPrice { get; set; }
        public string ProfitMargin { get; set; }
        public string InStockQuantity { get; set; }
        public string Category { get; set; }
        public string Brand { get; set; }
    }
}
