using IMS.BusinessRules;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Configuration;
using System.Security.Claims;
using System.Threading.Tasks;

namespace IMS.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit https://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    //change bellow classes
    public class ApplicationUser : IdentityUser<long, UserLoginLongPk, UserRoleLongPk, UserClaimLongPk>
    {
        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(ApplicationUserManager manager)
        {
            var userIdentity = await manager.CreateIdentityAsync(this,
                               DefaultAuthenticationTypes.ApplicationCookie);

            // Add custom user claims here
            return userIdentity;
        }
    }

    //string connectionString = DbConnectionString.ConnectionString;
    //public ApplicationDbContext()
    //    : base("Data Source = DESKTOP-L0GNHBL\\SQLEXPRESS;Database=IMS;Trusted_Connection=True;")

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser, RoleLongPk, long,
        UserLoginLongPk, UserRoleLongPk, UserClaimLongPk>
    {     
        //"Data Source = DESKTOP-L0GNHBL\SQLEXPRESS;Database=IMS;Trusted_Connection=True;"
        public ApplicationDbContext()
            : base("Data Source = .\\SQLEXPRESS;Database=IMS;Trusted_Connection=True;")
        {

        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }
    }

    //New drived classes 
    public class UserRoleLongPk : IdentityUserRole<long>
    {
    }

    public class UserClaimLongPk : IdentityUserClaim<long>
    {
    }

    public class UserLoginLongPk : IdentityUserLogin<long>
    {
    }

    public class RoleLongPk : IdentityRole<long, UserRoleLongPk>
    {
        public RoleLongPk() { }
        public RoleLongPk(string name) { Name = name; }
    }

    public class UserStoreLongPk : UserStore<ApplicationUser, RoleLongPk, long,
       UserLoginLongPk, UserRoleLongPk, UserClaimLongPk>
    {
        public UserStoreLongPk(ApplicationDbContext context)
            : base(context)
        {
        }
    }

    public class RoleStoreLongPk : RoleStore<RoleLongPk, long, UserRoleLongPk>
    {
        public RoleStoreLongPk(ApplicationDbContext context)
            : base(context)
        {
        }
    }
}