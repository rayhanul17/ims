using System;

namespace IMS.BusinessModel.Entity
{
    public class Payment : IEntity<long>
    {
        public virtual long Id { get; set; }
        public virtual long OperationId { get; set; }
        public virtual int OperationType { get; set; }
        public virtual int PaymentMethod { get; set; }
        public virtual decimal Amount { get; set; }
        public virtual bool IsPaid { get; set; }
        public virtual string TransactionId { get; set; }
        public virtual DateTime PaymentDate { get; set; }
        public virtual long BankId { get; set; }
        public virtual Bank Bank { get; set; }
    }
}
