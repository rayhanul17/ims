using FluentNHibernate.Mapping;
using IMS.BusinessModel.Entity;

namespace IMS.Dao.Mappings
{
    public class SaleMapping : BaseMapping<Sale, long>
    {
        public SaleMapping()
        {
            Table("Sale");
            Map(x => x.CustomerId);
            Map(x => x.SaleDate);
            Map(x => x.GrandTotalPrice);
            Map(x => x.IsPaid);
            Map(x => x.PaymentId);

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
