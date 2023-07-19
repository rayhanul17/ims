using IMS.BusinessRules.Enum;
using System;
using System.Collections.Generic;
using System.Text;

namespace IMS.BusinessModel.Dto
{
    public abstract class BaseDto<T>
    {
        public virtual T Id { get; set; }
        public virtual string Name { get; set; }
        public virtual Status Status { get; set; }
        public virtual T CreateBy { get; set; }
        public virtual DateTime CreationDate { get; set; }
        public virtual T ModifyBy { get; set; }
        public virtual DateTime? ModificationDate { get; set; }
        public virtual int Rank { get; set; }
        public virtual int? VersionNumber { get; set; }
        public virtual string BusinessId { get; set; }
    }
}
