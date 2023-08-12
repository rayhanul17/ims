using FluentNHibernate.Mapping;
using IMS.BusinessModel.Entity;

namespace IMS.Dao.Mappings
{
    public class PurchaseMapping : BaseMapping<Purchase, long>
    {
        public PurchaseMapping()
        {
            Table("Purchase");           
            Map(x => x.SupplierId);            
            Map(x => x.PurchaseDate);
            Map(x => x.GrandTotalPrice);
            Map(x => x.IsPaid);
            Map(x => x.PaymentId);

            HasMany(x => x.PurchaseDetails)
                .KeyColumn("PurchaseId")                
                .LazyLoad()
                .Cascade.All();
        }
    }

    public class PurchaseDetailsMapping : ClassMap<PurchaseDetails>
    {
        public PurchaseDetailsMapping()
        {
            Table("PurchaseDetails");
            Id(x => x.Id);
            Map(x => x.ProductId);
            Map(x => x.Quantity);
            Map(x => x.TotalPrice);

            References(i => i.Purchase)
               .Column("PurchaseId");
        }
    }
}
