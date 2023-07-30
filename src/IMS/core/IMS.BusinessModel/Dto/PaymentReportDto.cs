using IMS.BusinessRules.Enum;
using System;
using System.Collections.Generic;

namespace IMS.BusinessModel.Dto
{
    public class PaymentReportDto
    {
        public long Id { get; set; }
        public long OperationId { get; set; }
        public OperationType OperationType { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal PaidAmount { get; set; }
        public decimal DueAmount { get; set; }
        public IList<PaymentInformation> PaymentDetails { get; set; } = new List<PaymentInformation>();
    }

    public class PaymentInformation
    {
        public long Id { get; set; }
        public PaymentMethod PaymentMethod { get; set; }
        public decimal Amount { get; set; } 
        public string TransactionId { get; set; }
        public DateTime PaymentDate { get; set; }
        public long BankId { get; set; }
        public string Bank { get; set; }
    }
}
