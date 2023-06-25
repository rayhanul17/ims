using FluentNHibernate.Mapping;

namespace ConsoleApp1.Models;

public class ProductMap : ClassMap<Product>
{
    public ProductMap()
    {
        Table("Products");
        Id(i => i.Id);
        Map(i => i.Name);
        Map(i => i.Price);
        References(i => i.Category)
            .Column("CategoryId")
            .Cascade.All();
    }
}