using IMS.BusinessRules.Enum;

namespace IMS.BusinessModel.ViewModel
{
    public class ProductAddModel
    {
        public string Name { get; set; }
        public decimal Price { get; set; }
        public long CategoryId { get; set; }
        public string Description { get; set; }
        public int Rank { get; set; }
        public Status Status { get; set; }
    }
}
