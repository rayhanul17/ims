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
        public IList<(string ProductName, int Quantity, decimal TotalPrice)> Products { get; set; }

        public PurchaseReportDto()
        {
            Products = new List<(string ProductName, int Quantity, decimal TotalPrice)>();
        }
    }
}
