using IMS.BusinessRules.Enum;
using System;

namespace IMS.BusinessModel.Dto
{
    public class ProductDto : BaseDto<long>
    {        
        public string Description { get; set; }
        public string Image { get; set; }
        public decimal BuyingPrice { get; set; }
        public decimal SellingPrice { get; set; }
        public decimal DiscountPrice { get; set; }
        public decimal ProfitMargin { get; set; }
        public int InStockQuantity { get; set; }
        public string Category { get; set; } 
        public string Brand { get; set; } 
    }
}
