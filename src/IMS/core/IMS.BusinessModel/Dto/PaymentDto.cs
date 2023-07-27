using IMS.BusinessRules.Enum;
using System;

namespace IMS.BusinessModel.Dto
{
    public class PaymentDto
    {
        public long Id { get; set; }
        public long OperationId { get; set; }
        public long BankId { get; set; }
        public OperationType OperationType { get; set; }
        public PaymentMethod PaymentMethod { get; set; }
        public decimal Amount { get; set; }
        public bool IsPaid { get; set; }
        public string TransactionId { get; set; }
        public DateTime PaymentDate { get; set; }
    }
}
