using FluentNHibernate.Mapping;
using IMS.BusinessModel.Entity;

namespace IMS.Dao.Mappings
{
    public class PaymentMapping : ClassMap<Payment>
    {
        public PaymentMapping()
        {
            Table("Payment");
            Id(x => x.Id).Not.Nullable();
            Map(x => x.OperationId);
            Map(x => x.OperationType);
            Map(x => x.PaymentMethod);
            Map(x => x.TransactionId);           
            Map(x => x.Amount).Not.Nullable();            
            Map(x => x.IsPaid);           
            Map(x => x.PaymentDate);           
            References(i => i.Bank)
                .Column("BankId").Not.Nullable();            

        }
    }
}
