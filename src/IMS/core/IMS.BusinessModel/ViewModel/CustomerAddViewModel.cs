using IMS.BusinessRules.Enum;
using System.ComponentModel.DataAnnotations;

namespace IMS.BusinessModel.ViewModel
{
    public class CustomerAddViewModel
    {
        [Required, MinLength(3), MaxLength(100)]
        public string Name { get; set; }

        [Required, MinLength(10), MaxLength(200)]
        public string Address { get; set; }

        [Required, MinLength(8), MaxLength(30)]
        public string ContactNumber { get; set; }

        [Required, EmailAddress, MaxLength(254)]
        public string Email { get; set; }

        [Required]
        public Status Status { get; set; }
    }
}
