﻿namespace IMS.BusinessModel.ViewModel
{
    public class PurchaseDetailsModel
    {  
        public bool IsDeleted { get; set; }
        public long ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal Total { get; set; }
    }
}
