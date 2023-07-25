namespace IMS.BusinessModel.Entity
{
    public class PurchaseDetails : IEntity<long>
    {
        public virtual long Id { get; set; }
        public virtual long ProductId { get; set; }
        public virtual int Quantity { get; set; }
        public virtual decimal TotalPrice { get; set; }
        public virtual long PurchaseId { get; set; }
        public virtual Purchase Purchase { get; set; }
    }
}
