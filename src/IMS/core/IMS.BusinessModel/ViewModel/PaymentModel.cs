using IMS.BusinessRules.Enum;
using System;
using System.ComponentModel.DataAnnotations;

namespace IMS.BusinessModel.ViewModel
{
    public class PaymentModel
    {
        public long Id { get; set; }
        public long PaymentId { get; set; }

        [Required]
        public long BankId { get; set; }
        public PaymentMethod PaymentMethod { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal DueAmount { get; set; }
        [Required]
        public decimal Amount { get; set; }
        public string TransactionId { get; set; }
        public DateTime PaymentDate { get; set; }
    }
}
