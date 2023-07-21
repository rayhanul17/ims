using IMS.BusinessRules.Enum;
using System;

namespace IMS.BusinessModel.ViewModel
{
    public class ProductEditModel
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }  
        public long CategoryId { get; set; }
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
