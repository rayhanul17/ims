using System.Collections.Generic;

namespace IMS.BusinessModel.Dto
{
    public class PaymentReportDto
    {
        public string Id { get; set; }
        public string OperationId { get; set; }
        public string OperationType { get; set; }
        public string TotalAmount { get; set; }
        public string PaidAmount { get; set; }
        public string DueAmount { get; set; }
        public IList<PaymentInformation> PaymentDetails { get; set; } = new List<PaymentInformation>();
    }

    public class PaymentInformation
    {
        public string Id { get; set; }
        public string PaymentMethod { get; set; }
        public string Amount { get; set; }
        public string TransactionId { get; set; }
        public string PaymentDate { get; set; }
        public string BankId { get; set; }
        public string Bank { get; set; }
    }
}
