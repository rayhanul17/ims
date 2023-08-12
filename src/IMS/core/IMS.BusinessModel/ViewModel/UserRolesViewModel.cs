using System.Collections.Generic;

namespace IMS.BusinessModel.ViewModel
{
    public class UserRolesViewModel
    {
        public long UserId { get; set; }
        public List<Role> Roles { get; set; }
    }

    public class Role
    {
        public string Value { get; set; }
        public string Text { get; set; }
        public bool IsChecked { get; set; }
    }    
}
