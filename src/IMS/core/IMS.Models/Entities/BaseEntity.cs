using IMS.Infrastructure.Enums;
using System;

namespace IMS.Models.Entities
{
    public abstract class BaseEntity : IEntity<long>
    {
        public virtual long Id { get; set; }
        public virtual long Status { get; set; }
        public virtual long CreateBy { get; set; }
        public virtual DateTime CreationDate { get; set; }
        public virtual long ModifyBy { get; set; }
        public virtual DateTime ModificationDate { get; set; }
        public virtual int Rank { get; set; }
        public virtual int VersionNumber { get; set; }
        public virtual string BusinessId { get; set; }
    }
}
