using IMS.BusinessRules.Enum;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace IMS.BusinessModel.ViewModel
{
    public class BrandAddModel
    {
        [Required, MinLength(3)]
        public string Name { get; set; }
        public string Description { get; set; }
        public long Rank { get; set; }

        [Required]
        public Status Status { get; set; }

    }
}
