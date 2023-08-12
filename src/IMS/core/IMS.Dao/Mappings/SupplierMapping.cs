using IMS.BusinessModel.Entity;

namespace IMS.Dao.Mappings
{
    public class SupplierMapping : BaseMapping<Supplier, long>
    {
        public SupplierMapping()
        {
            Table("Supplier");
            Map(x => x.Name).Not.Nullable();
            Map(x => x.Address);
            Map(x => x.ContactNumber);
            Map(x => x.Email);
        }
    }
}
