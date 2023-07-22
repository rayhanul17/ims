﻿using IMS.BusinessModel.Entity;

namespace IMS.Dao.Mappings
{
    public class ProductMapping : BaseMapping<Product, long>
    {
        public ProductMapping() : base()
        {
            Table("Product");
            Map(x => x.Description);
            Map(x => x.BuyingPrice);
            Map(x => x.SellingPrice);
            Map(x => x.DiscountPrice);           
            Map(x => x.ProfitMargin).Not.Nullable();            
            Map(x => x.Image);
            Map(x => x.InStockQuantity);           
            References(i => i.Category)
                .Column("CategoryId").Not.Nullable();
            
        }
    }
}