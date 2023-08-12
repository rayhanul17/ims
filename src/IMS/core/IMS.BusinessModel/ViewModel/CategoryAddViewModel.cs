using IMS.BusinessRules.Enum;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace IMS.BusinessModel.ViewModel
{
    public class CategoryAddViewModel
    {
        [Required, MinLength(3)]
        public string Name { get; set; }

        [Required, MinLength(3)]
        public string Description { get; set; }

        [Required]
        public Status Status { get; set; }

    }
}
