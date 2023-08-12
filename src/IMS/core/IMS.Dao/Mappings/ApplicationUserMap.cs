using IMS.BusinessModel.Entity;

namespace IMS.Dao.Mappings
{
    public class ApplicationUserMap : BaseMapping<ApplicationUser, long>
    {
        public ApplicationUserMap() : base()
        {
            Table("ApplicationUser");
            Map(x => x.Name).Not.Nullable();
            Map(x => x.AspNetUsersId);
            Map(x => x.Email);
        }
    }
}
