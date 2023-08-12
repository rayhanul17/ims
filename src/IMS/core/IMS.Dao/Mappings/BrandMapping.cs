using IMS.BusinessModel.Entity;

namespace IMS.Dao.Mappings
{
    public class BrandMapping : BaseMapping<Brand, long>
    {
        public BrandMapping() : base()
        {
            Table("Brand");
            Map(x => x.Name).Not.Nullable();
            Map(x => x.Description);

            HasMany(x => x.Products)
                .KeyColumn("BrandId")
                .Inverse()
                .LazyLoad()
                .Cascade.All();
        }
    }
}
