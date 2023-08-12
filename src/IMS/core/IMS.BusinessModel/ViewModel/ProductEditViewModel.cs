﻿using IMS.BusinessRules.Enum;
using System;

namespace IMS.BusinessModel.ViewModel
{
    public class ProductEditViewModel
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public virtual string Image { get; set; }
        public virtual decimal BuyingPrice { get; set; }
        public virtual decimal SellingPrice { get; set; }
        public virtual decimal DiscountPrice { get; set; }
        public virtual decimal UnitPrice { get; set; }
        public virtual decimal ProfitMargin { get; set; }
        public virtual int InStockQuantity { get; set; }
        public long CategoryId { get; set; }
        public long BrandId { get; set; }
        public string Description { get; set; }
        public virtual Status Status { get; set; }
        public virtual long CreateBy { get; set; }
        public virtual DateTime CreationDate { get; set; }
        public virtual long ModifyBy { get; set; }
        public virtual DateTime? ModificationDate { get; set; }
        public virtual long Rank { get; set; }
        public virtual long? VersionNumber { get; set; }
        public virtual string BusinessId { get; set; }

    }
}