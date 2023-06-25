using FluentNHibernate.Mapping;

namespace ConsoleApp1.Models;

public class CategoryMap : ClassMap<Category>
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