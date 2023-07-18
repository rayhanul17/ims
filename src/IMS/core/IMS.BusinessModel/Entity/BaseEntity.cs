using System;

namespace IMS.BusinessModel.Entity
{
    public abstract class BaseEntity<T> : IEntity<T>
    {
        public virtual T Id { get; set; }
        public virtual string Name { get; set; }
        public virtual int Status { get; set; }
        public virtual T CreateBy { get; set; }
        public virtual DateTime CreationDate { get; set; }        
        public virtual T ModifyBy { get; set; }
        public virtual DateTime? ModificationDate { get; set; }
        public virtual int Rank { get; set; }
        public virtual int? VersionNumber { get; set; }
        public virtual string BusinessId { get; set; }
    }
}
