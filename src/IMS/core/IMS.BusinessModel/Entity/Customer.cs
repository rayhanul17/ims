namespace IMS.BusinessModel.Entity
{
    public class Customer : BaseEntity<long>
    {
        public virtual string Name { get; set; }
        public virtual string Address { get; set; }
        public virtual string ContactNumber { get; set; }
        public virtual string Email { get; set; }
    }
}
