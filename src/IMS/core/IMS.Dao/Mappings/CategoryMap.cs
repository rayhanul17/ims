using FluentNHibernate.Mapping;
using IMS.BusinessModel.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMS.Dao.Mappings
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
            Map(x => x.CreateBy);
            Map(x => x.CreationDate);
            Map(x => x.ModifyBy).Nullable();
            Map(x => x.ModificationDate).Nullable();
            Map(x => x.Status);
            Map(x => x.Rank);
            Map(x => x.VersionNumber);
            Map(x => x.BusinessId).Nullable();
            //HasMany(x => x.Products)
            //    .KeyColumn("CategoryId")
            //    .Inverse()
            //    .LazyLoad()
            //    .Cascade.All();
        }
    }
}
