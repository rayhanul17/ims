using IMS.BusinessModel.Entity;

namespace IMS.Dao.Mappings
{
    public class BankMapping : BaseMapping<Bank, long>
    {
        public BankMapping() : base()
        {
            Table("Bank");            
            Map(x => x.Description);
            HasMany(x => x.Payment)
                .KeyColumn("BankId")
                .Inverse()
                .LazyLoad()
                .Cascade.All();
        }
    }
}
