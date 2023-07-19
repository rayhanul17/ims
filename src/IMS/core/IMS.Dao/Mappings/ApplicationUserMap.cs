using FluentNHibernate.Mapping;
using IMS.BusinessModel.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMS.Dao.Mappings
{
    public class ApplicationUserMap : BaseMapping<ApplicationUser, long>
    {
        public ApplicationUserMap() : base()
        {
            Table("ApplicationUser");
            Map(x => x.AspNetUsersId);
        }
    }
}
