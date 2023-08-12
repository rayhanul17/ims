using IMS.BusinessModel.Entity;

namespace IMS.Dao.Mappings
{
    public class CategoryMapping : BaseMapping<Category, long>
    {
        public CategoryMapping() : base()
        {
            Table("Category");
            Map(x => x.Name).Not.Nullable();
            Map(x => x.Description);

            HasMany(x => x.Products)
                .KeyColumn("CategoryId")
                .Inverse()
                .LazyLoad()
                .Cascade.All();
        }
    }
}
