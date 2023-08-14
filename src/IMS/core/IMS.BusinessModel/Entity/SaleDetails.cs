namespace IMS.BusinessModel.Entity
{
    public class SaleDetails : BaseEntity<long>
    {
        public virtual long ProductId { get; set; }
        public virtual int Quantity { get; set; }
        public virtual decimal TotalPrice { get; set; }
        public virtual Sale Sale { get; set; }
    }
}
