namespace IMS.BusinessModel.Entity
{
    public class SaleDetails : IEntity<long>
    {
        public virtual long Id { get; set; }
        public virtual long ProductId { get; set; }
        public virtual int Quantity { get; set; }
        public virtual decimal TotalPrice { get; set; }
        public virtual long SaleId { get; set; }
        public virtual Sale Sale { get; set; }
    }
}
