using IMS.BusinessRules.Enum;

namespace IMS.BusinessModel.ViewModel
{
    public class BankAddModel
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public int Rank { get; set; }

        public Status Status { get; set; }

    }
}
