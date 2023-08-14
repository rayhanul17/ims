using FluentNHibernate.Mapping;
using IMS.BusinessModel.Entity;

namespace IMS.Dao.Mappings
{
    public class PaymentMapping : BaseMapping<Payment, long>
    {
        public PaymentMapping()
        {
            Table("Payment");
            Map(x => x.PurchaseId);
            Map(x => x.SaleId);
            Map(x => x.VoucherId);
            Map(x => x.OperationType);
            Map(x => x.TotalAmount).Not.Nullable();
            Map(x => x.PaidAmount);
            HasMany(x => x.PaymentDetails)
                .KeyColumn("PaymentId")
                .LazyLoad()
                .Cascade.All();
        }
    }

    public class PaymentDetailsMapping : BaseMapping<PaymentDetails, long>
    {
        public PaymentDetailsMapping()
        {
            Map(x => x.PaymentMethod);
            Map(x => x.TransactionId);
            Map(x => x.Amount).Not.Nullable();
            Map(x => x.PaymentDate).Not.Nullable();

            References(i => i.Payment)
                .Column("PaymentId").Not.Nullable();

            References(i => i.Bank)
                .Column("BankId").Nullable();
        }
    }
}
