namespace IMS.BusinessModel.Entity
{
    public class ApplicationUser : BaseEntity<long>
    {
        public virtual string Name { get; set; }
        public virtual long AspNetUsersId { get; set; }
        public virtual string Email { get; set; }
    }
}
