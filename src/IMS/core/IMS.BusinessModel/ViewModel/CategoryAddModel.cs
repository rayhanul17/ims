using IMS.BusinessRules.Enum;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace IMS.BusinessModel.ViewModel
{
    public class CategoryAddModel
    {
        [Required, MinLength(3)]
        public string Name { get; set; }
        public string Description { get; set; }
        public int Rank { get; set; }

        [Required]
        public Status Status { get; set; }

    }
}
