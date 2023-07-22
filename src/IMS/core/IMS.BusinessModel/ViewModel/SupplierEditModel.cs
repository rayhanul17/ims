using IMS.BusinessRules.Enum;
using System;
using System.ComponentModel.DataAnnotations;

namespace IMS.BusinessModel.ViewModel
{
    public class SupplierEditModel
    {
        public long Id { get; set; }

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
        public virtual long CreateBy { get; set; }
        public virtual DateTime CreationDate { get; set; }
        public virtual long ModifyBy { get; set; }
        public virtual DateTime? ModificationDate { get; set; }
        public virtual int Rank { get; set; }
        public virtual int? VersionNumber { get; set; }
        public virtual string BusinessId { get; set; }

    }
}
