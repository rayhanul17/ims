﻿using FluentNHibernate.Mapping;
using IMS.BusinessModel.Entity;

namespace IMS.Dao.Mappings
{
    public class PurchaseMapping : ClassMap<Purchase>
    {
        public PurchaseMapping()
        {
            Table("Purchase");
            Id(x => x.Id);
            Map(x => x.SupplierId);
            Map(x => x.CreateBy);
            Map(x => x.PurchaseDate);
            Map(x => x.GrandTotalPrice);

            HasMany(x => x.PurchaseDetails)
                .KeyColumn("PurchaseId")                
                .LazyLoad()
                .Cascade.All();
        }
    }

    public class PurchaseDetailsMapping : ClassMap<PurchaseDetails>
    {
        public PurchaseDetailsMapping()
        {
            Table("PurchaseDetails");
            Id(x => x.Id);
            Map(x => x.ProductId);
            Map(x => x.Quantity);
            Map(x => x.TotalPrice);

            References(i => i.Purchase)
               .Column("PurchaseId");
        }
    }
}