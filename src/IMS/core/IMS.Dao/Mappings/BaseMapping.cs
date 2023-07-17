using FluentNHibernate.Mapping;
using IMS.BusinessModel.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMS.Dao.Mappings
{
    public abstract class BaseMapping<TEntity, TKey> : ClassMap<TEntity>
        where TEntity : BaseEntity<TKey>
    {
        public BaseMapping()
        {
            Table("Category");
            Id(x => x.Id);
            Map(x => x.Name).Not.Nullable();            
            Map(x => x.CreateBy).Not.Nullable();
            Map(x => x.CreationDate).Not.Nullable();
            Map(x => x.ModifyBy);
            Map(x => x.ModificationDate);
            Map(x => x.Status).Not.Nullable();
            Map(x => x.Rank);
            Map(x => x.VersionNumber);
            Map(x => x.BusinessId);
        }
    }
}
