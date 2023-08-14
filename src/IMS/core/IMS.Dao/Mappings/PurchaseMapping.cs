using IMS.BusinessModel.Entity;

namespace IMS.Dao.Mappings
{
    public class PurchaseMapping : BaseMapping<Purchase, long>
    {
        public PurchaseMapping()
        {
            Map(x => x.VoucherId);
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

    public class PurchaseDetailsMapping : BaseMapping<PurchaseDetails, long>
    {
        public PurchaseDetailsMapping()
        {
            Table(typeof(PurchaseDetails).Name);
            Map(x => x.ProductId);
            Map(x => x.Quantity);
            Map(x => x.TotalPrice);

            References(i => i.Purchase)
               .Column("PurchaseId");
        }
    }
}
