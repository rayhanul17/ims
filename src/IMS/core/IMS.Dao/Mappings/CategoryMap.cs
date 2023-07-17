using FluentNHibernate.Mapping;
using IMS.BusinessModel.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMS.Dao.Mappings
{
    public class CategoryMap : BaseMapping<Category, long>
    {
        public CategoryMap() : base()
        {
            Table("Category");
            Map(x => x.Description);
            
            //HasMany(x => x.Products)
            //    .KeyColumn("CategoryId")
            //    .Inverse()
            //    .LazyLoad()
            //    .Cascade.All();
        }
    }
}
