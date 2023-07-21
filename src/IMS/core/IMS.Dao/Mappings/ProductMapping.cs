using IMS.BusinessModel.Entity;

namespace IMS.Dao.Mappings
{
    public class ProductMapping : BaseMapping<Product, long>
    {
        public ProductMapping() : base()
        {
            Table("Product");
            Map(x => x.Description);
            Map(x => x.Price).Not.Nullable();            
            References(i => i.Category)
                .Column("CategoryId").Not.Nullable();
            
        }
    }
}
