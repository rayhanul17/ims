using IMS.BusinessRules.Enum;
using System.ComponentModel.DataAnnotations;

namespace IMS.Models
{
    public class CategoryAddModelTest
    {
        [Required, MinLength(3)]
        public string Name { get; set; }
        public string Description { get; set; }
        public int Rank { get; set; }

        [Required]
        public Status Status { get; set; }
    }
}