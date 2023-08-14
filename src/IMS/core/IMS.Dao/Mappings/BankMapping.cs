﻿using IMS.BusinessModel.Entity;

namespace IMS.Dao.Mappings
{
    public class BankMapping : BaseMapping<Bank, long>
    {
        public BankMapping() : base()
        {
            Map(x => x.Name).Not.Nullable();
            Map(x => x.Description).Length(4001);
            HasMany(x => x.PaymentDetails)
                .KeyColumn("BankId")
                .Inverse()
                .LazyLoad()
                .Cascade.All();
        }
    }
}
