namespace IMS.BusinessModel.Entity
{
    public class Product : BaseEntity<long>
    {
        public virtual string Description { get; set; }  
        public virtual string Image { get; set; }
        public virtual decimal BuyingPrice { get; set; }
        public virtual decimal SellingPrice { get; set; }
        public virtual decimal DiscountPrice { get; set; }
        public virtual decimal ProfitMargin { get; set; }
        public virtual int InStockQuantity { get; set; }
        public virtual long CategoryId { get; set; }
        public virtual Category Category { get; set; }
    }
}
