using IMS.BusinessRules.Enum;
using System.ComponentModel.DataAnnotations;

namespace IMS.BusinessModel.ViewModel
{
    public class ProductAddModel
    {
        public string Name { get; set; }
        public string Image { get; set; }
        public decimal BuyingPrice { get; set; }
        public decimal SellingPrice { get; set; }
        public decimal DiscountPrice { get; set; }
        [Required]
        public decimal ProfitMargin { get; set; }
        public int InStockQuantity { get; set; }
        public long CategoryId { get; set; }
        public long BrandId { get; set; }
        public string Description { get; set; }
        public long Rank { get; set; }
        public Status Status { get; set; }
    }
}
