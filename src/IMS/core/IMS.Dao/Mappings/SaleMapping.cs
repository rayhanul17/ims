using FluentNHibernate.Mapping;
using IMS.BusinessModel.Entity;

namespace IMS.Dao.Mappings
{
    public class SaleMapping : ClassMap<Sale>
    {
        public SaleMapping()
        {
            Table("Sale");
            Id(x => x.Id);
            Map(x => x.CustomerId);
            Map(x => x.CreateBy);
            Map(x => x.SaleDate);
            Map(x => x.GrandTotalPrice);

            HasMany(x => x.SaleDetails)
                .KeyColumn("SaleId")                
                .LazyLoad()
                .Cascade.All();
        }
    }

    public class SaleDetailsMapping : ClassMap<SaleDetails>
    {
        public SaleDetailsMapping()
        {
            Table("SaleDetails");
            Id(x => x.Id);
            Map(x => x.ProductId);
            Map(x => x.Quantity);
            Map(x => x.TotalPrice);

            References(i => i.Sale)
               .Column("SaleId");
        }
    }
}
