using FluentNHibernate.Mapping;
using Test.Models;

namespace Test.Mappings
{
    public class CategoryMap : ClassMap<CategoryModel>
    {
        public CategoryMap()
        {
            Table("Categories");
            Id(x => x.Id).GeneratedBy.Identity();
            Map(x => x.Name);
            HasMany(x => x.Products)
                .Inverse()
                .LazyLoad()
                .Cascade.SaveUpdate()
                .KeyColumn("CategoryId");
        }
    }
}