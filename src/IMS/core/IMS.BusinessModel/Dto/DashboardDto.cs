using System;

namespace IMS.BusinessModel.Dto
{
    public class DashboardDto
    {
        public decimal ToatlPurchaseAmount{ get; set; }
        public decimal ToatlPurchasePaidAmount{ get; set; }
        public decimal ToatlPurchaseDueAmount{ get; set; }
        public decimal TotalSaleAmount { get; set; }
        public decimal TotalSalePaidAmount { get; set; }
        public decimal TotalSaleDueAmount { get; set; }
        public int TotalActiveCustomer { get; set; }
        public int TotalInActiveCustomer { get; set; }
        public int TotalActiveSupplier { get; set; }
        public int TotalInActiveSupplier { get; set; }
    }

    public class MyModel
    {
        public long SupplierId { get; set; }
        public decimal GrandTotalPrice { get; set; }
        public DateTime PurchaseDate { get; set; }
    }

    public class PurchaseSaleAmountDto
    {
        public int OperationType { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal TotalPaidAmount { get; set; }
        public decimal TotalDueAmount { get; set; }
    }

    public class ActiveInactiveDto
    {
        public int Status { get; set;}
        public int Count { get; set;}
    }
}
