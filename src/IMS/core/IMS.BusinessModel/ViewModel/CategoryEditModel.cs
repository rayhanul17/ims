using IMS.BusinessRules.Enum;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace IMS.BusinessModel.ViewModel
{
    public class CategoryEditModel
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public virtual Status Status { get; set; }
        public virtual long CreateBy { get; set; }
        public virtual string CreationDate { get; set; }
        public virtual long ModifyBy { get; set; }
        public virtual DateTime? ModificationDate { get; set; }
        public virtual int Rank { get; set; }
        public virtual int? VersionNumber { get; set; }
        public virtual string BusinessId { get; set; }

    }
}
