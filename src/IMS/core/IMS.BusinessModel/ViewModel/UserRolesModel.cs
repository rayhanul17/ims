using System.Collections.Generic;

namespace IMS.BusinessModel.ViewModel
{
    public class UserRolesModel
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

    public class CityModel
    {
        //Value of checkbox 
        public int Value { get; set; }
        //description of checkbox 
        public string Text { get; set; }
        //whether the checkbox is selected or not
        public bool IsChecked { get; set; }
    }
    public class CityList
    {
        //use CheckBoxModel class as list 
        public List<CityModel> Cities { get; set; }
    }
}
