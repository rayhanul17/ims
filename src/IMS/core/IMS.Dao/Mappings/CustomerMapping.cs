using IMS.BusinessModel.Entity;

namespace IMS.Dao.Mappings
{
    public class CustomerMapping : BaseMapping<Customer, long>
    {
        public CustomerMapping()
        {
            Table("Customer");
            Map(x => x.Address);
            Map(x => x.ContactNumber);
            Map(x => x.Email);
        }
    }
}
