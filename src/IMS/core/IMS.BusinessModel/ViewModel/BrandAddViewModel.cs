﻿using IMS.BusinessRules.Enum;
using System.ComponentModel.DataAnnotations;

namespace IMS.BusinessModel.ViewModel
{
    public class BrandAddViewModel
    {
        [Required, MinLength(3)]
        public string Name { get; set; }
        public string Description { get; set; }

        [Required]
        public Status Status { get; set; }

    }
}
