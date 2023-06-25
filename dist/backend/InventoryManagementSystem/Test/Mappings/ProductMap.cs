using FluentNHibernate.Mapping;
using Test.Models;

namespace Test.Mappings
{
    public class ProductMap : ClassMap<ProductModel>
    {
        public ProductMap()
        {
            Table("Products");
            Id(i => i.Id);
            Map(i => i.Name);
            Map(i => i.Price);
            References(i => i.Category)
                .Column("CategoryId");                
        }
    }
}