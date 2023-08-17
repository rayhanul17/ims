using System;
using System.Collections.Generic;

namespace IMS.BusinessModel.Dto
{
    public class LoseProfitReportDto
    {
        public string DateRange { get; set; }
        public PaymentDetailsDto PurchasePaymentDetails { get; set; } = new PaymentDetailsDto();
        public PaymentDetailsDto SalePaymentDetails { get; set; } = new PaymentDetailsDto();
        public IList<ProductListDto> PurchaseProductList { get; set; } = new List<ProductListDto>();
        public IList<ProductListDto> SaleProductList { get; set; } = new List<ProductListDto>();
    }

    public class BuyingSellingReportDto
    {
        public long? CategoryId { get; set; }
        public long? BrandId { get; set; }
        public long? ProductId { get; set; }
        public long? SupplierId { get; set; }
        public long? CustomerId { get; set; }
        public string DateRange { get; set; }
        public decimal? PuchaseAmount { get; set; }
        public decimal? SaleAmount { get; set; }
        public IList<ProductListDto> PurchaseProductList { get; set; } = new List<ProductListDto>();
        public IList<ProductListDto> SaleProductList { get; set; } = new List<ProductListDto>();
    }
    public class PaymentDetailsDto
    {
        public int Count { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal PaidAmount { get; set; }
        public decimal DueAmount { get; set; }
    }

    public class ProductListDto
    {
        public string Name { get; set; }
        public int Quantity { get; set; }
        public decimal TotalPrice { get; set; }
        public DateTime Date { get; set; }
    }
}
