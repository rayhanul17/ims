using System;
using System.Collections.Generic;
using System.Text;

namespace IMS.BusinessModel.Entity
{
    public class ApplicationUser : BaseEntity<long>
    {
        public virtual long AspNetUsersId { get; set; }  
        public virtual string Email { get; set;}
    }
}
