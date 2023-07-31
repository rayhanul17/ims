using System;
using System.Collections.Generic;

namespace IMS.BusinessModel.Dto
{
    public class SaleReportDto
    {
        public string CustomerName { get; set; }
        public string CustomerDescription { get; set; }
        public DateTime SaleDate { get; set; }
        public decimal GrandTotalPrice { get; set; }
        public long PaymentId { get; set; }
        public long SaleId { get; set; }
        public IList<ProductInformation> Products { get; set; } = new List<ProductInformation>();

    }
}
