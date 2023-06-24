using FluentNHibernate.Mapping;
using System.Web.UI.WebControls;
using Test.Models;

namespace Test.Mappings
{
    public class ProductMap : ClassMap<ProductModel>
    {
        public ProductMap()
        {
            Table("Products");
            Map(i => i.Id);
            Map(i => i.Name);
            Map(i => i.Price);
        }
    }
}