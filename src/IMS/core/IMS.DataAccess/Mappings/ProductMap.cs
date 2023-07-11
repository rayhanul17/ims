using FluentNHibernate.Mapping;
using IMS.Models.Entities;

namespace IMS.DataAccess.Mappings
{
    public class ProductMap : ClassMap<Product>
    {
        public ProductMap()
        {
            Table("Product");
            Id(x => x.Id);
            Map(x => x.Name);
            Map(x => x.Description);
            References(x => x.Category)
                .Column("CategoryId");
        }
    }
}
