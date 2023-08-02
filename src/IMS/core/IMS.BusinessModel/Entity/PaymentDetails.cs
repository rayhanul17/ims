using System;

namespace IMS.BusinessModel.Entity
{
    public class PaymentDetails : IEntity<long>
    {
        public virtual long Id { get; set; }
        public virtual int PaymentMethod { get; set; }
        public virtual decimal Amount { get; set; }
        public virtual string TransactionId { get; set; }
        public virtual DateTime PaymentDate { get; set; }
        public virtual long? BankId { get; set; }
        public virtual Bank Bank { get; set; }
        public virtual long PaymentId { get; set; }
        public virtual Payment Payment { get; set; }
    }
}
