using IMS.BusinessRules.Enum;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace IMS.BusinessModel.ViewModel
{
    public class SupplierAddModel
    {
        [Required, MinLength(3), MaxLength(100)]
        public string Name { get; set; }

        [Required, MinLength(10), MaxLength(200)]
        public string Address { get; set; }

        [Required, MinLength(8), MaxLength(30)]
        public string ContactNumber { get; set; }

        [Required, EmailAddress, MaxLength(254)]
        public string Email { get; set; }
        public long Rank { get; set; }

        [Required]
        public Status Status { get; set; }
    }
}
