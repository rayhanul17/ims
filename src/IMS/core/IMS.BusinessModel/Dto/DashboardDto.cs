using System;

namespace IMS.BusinessModel.Dto
{
    public class DashboardDto
    {
        public decimal ToatlPurchasePrice { get; set; }
        public decimal TotalSalePrice { get; set; }
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
}
