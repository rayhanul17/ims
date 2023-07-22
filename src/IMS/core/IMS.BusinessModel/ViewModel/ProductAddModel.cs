using IMS.BusinessRules.Enum;

namespace IMS.BusinessModel.ViewModel
{
    public class ProductAddModel
    {
        public string Name { get; set; }
        public virtual string Image { get; set; }
        public virtual decimal BuyingPrice { get; set; }
        public virtual decimal SellingPrice { get; set; }
        public virtual decimal DiscountPrice { get; set; }
        public virtual decimal ProfitMargin { get; set; }
        public virtual int InStockQuantity { get; set; }
        public long CategoryId { get; set; }
        public string Description { get; set; }
        public int Rank { get; set; }
        public Status Status { get; set; }
    }
}
