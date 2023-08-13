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
        public long PaymentId { get; set; }
        public long PurchaseId { get; set; }
        public IList<ProductInformation> Products { get; set; } = new List<ProductInformation>();

    }
}
