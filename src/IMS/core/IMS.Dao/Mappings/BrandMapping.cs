using IMS.BusinessModel.Entity;

namespace IMS.Dao.Mappings
{
    public class BrandMapping : BaseMapping<Brand, long>
    {
        public BrandMapping() : base()
        {
            Table("Brand");
            Map(x => x.Description);

            //HasMany(x => x.Products)
            //    .KeyColumn("CategoryId")
            //    .Inverse()
            //    .LazyLoad()
            //    .Cascade.All();
        }
    }
}
