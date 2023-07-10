using FluentNHibernate.Mapping;
using IMS.Models.Entities;

namespace IMS.DataAccess.Mappings
{
    public class CategoryMap : ClassMap<Category>
    {
        public CategoryMap()
        {
            Table("Category");
            Id(x => x.Id);
            Map(x => x.Name);
            Map(x => x.Description);
        }
    }
}
