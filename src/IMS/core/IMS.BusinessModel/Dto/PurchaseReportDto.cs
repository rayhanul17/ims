using System;
using System.Collections.Generic;

namespace IMS.BusinessModel.Dto
{
    public class PurchaseReportDto
    {
        public string SupplierName { get; set; }
        public string SupplierDescription { get; set; }
        public DateTime PurchaseDate { get; set; }
        public decimal GrandTotalPrice { get; set; }
        public IList<ProductInformation> Products { get; set; } = new List<ProductInformation>();

    }

    public class ProductInformation
    {
        public string ProductName { get; set; }
        public string Description { get; set; }
        public decimal UnitPrice { get; set; }
        public int Quantity { get; set; }
        public decimal TotalPrice { get; set; }
    }
}
