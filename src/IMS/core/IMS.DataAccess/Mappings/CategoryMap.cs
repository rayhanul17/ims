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
            Map(x => x.CreateBy);
            Map(x => x.Name);
            Map(x => x.Description);
            Map(x => x.CreationDate);
            Map(x => x.ModifyBy);
            Map(x => x.ModificationDate);
            Map(x => x.Status);
            Map(x => x.Rank);
            Map(x => x.VersionNumber);
            Map(x => x.BusinessId);
        }
    }
}
