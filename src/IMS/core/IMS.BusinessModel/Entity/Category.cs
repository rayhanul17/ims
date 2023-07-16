namespace IMS.BusinessModel.Entity
{
    public class Category : BaseEntity<long>
    {
        public virtual string Name { get; set; }
        public virtual string Description { get; set; }

        //public virtual IList<Product> Products { get; set; } = new List<Product>();
    }
}
