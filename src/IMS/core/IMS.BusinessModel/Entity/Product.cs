namespace IMS.BusinessModel.Entity
{
    public class Product : BaseEntity<long>
    {
        public virtual string Description { get; set; }
        public virtual decimal? Price { get; set; }    
        public virtual long CategoryId { get; set; }
        public virtual Category Category { get; set; }
    }
}
