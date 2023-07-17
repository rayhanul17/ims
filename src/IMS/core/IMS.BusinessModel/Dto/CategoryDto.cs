using IMS.BusinessRules.Enum;
using System;

namespace IMS.BusinessModel.Dto
{
    public class CategoryDto
    {
        public virtual long Id { get; set; }
        public virtual string Name { get; set; }
        public virtual string Description { get; set; }
        public virtual Status Status { get; set; }
        public virtual long CreateBy { get; set; }
        public virtual DateTime CreationDate { get; set; }
        public virtual long ModifyBy { get; set; }
        public virtual DateTime? ModificationDate { get; set; }
        public virtual int? Rank { get; set; }
        public virtual int? VersionNumber { get; set; }
        public virtual string BusinessId { get; set; }
    }
}
