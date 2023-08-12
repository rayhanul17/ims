using FluentNHibernate.Mapping;
using IMS.BusinessModel.Entity;

namespace IMS.Dao.Mappings
{
    public abstract class BaseMapping<TEntity, TKey> : ClassMap<TEntity>
        where TEntity : BaseEntity<TKey>
    {
        public BaseMapping()
        {
            Id(x => x.Id).Not.Nullable();
            Map(x => x.CreateBy).Not.Nullable();
            Map(x => x.CreationDate).Not.Nullable();
            Map(x => x.ModifyBy).Nullable();
            Map(x => x.ModificationDate).Nullable();
            Map(x => x.Status).Not.Nullable();
            Map(x => x.Rank).Nullable();
            Map(x => x.VersionNumber).Nullable();
            Map(x => x.BusinessId).Nullable();
        }
    }
}
