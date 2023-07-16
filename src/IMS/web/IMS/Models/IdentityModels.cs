using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Configuration;
using System.Security.Claims;
using System.Threading.Tasks;

namespace IMS.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit https://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    //change bellow classes
    public class ApplicationUser : IdentityUser<long, UserLoginIntPk, UserRoleIntPk, UserClaimIntPk>
    {
        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(ApplicationUserManager manager)
        {
            var userIdentity = await manager.CreateIdentityAsync(this,
                               DefaultAuthenticationTypes.ApplicationCookie);

            // Add custom user claims here
            return userIdentity;
        }
    }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser, RoleIntPk, long,
        UserLoginIntPk, UserRoleIntPk, UserClaimIntPk>
    {
        string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
        public ApplicationDbContext()
            : base("Data Source = DESKTOP-L0GNHBL\\SQLEXPRESS;Database=IMS;Trusted_Connection=True;")
        {
        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }
    }

    //New drived classes 
    public class UserRoleIntPk : IdentityUserRole<long>
    {
    }

    public class UserClaimIntPk : IdentityUserClaim<long>
    {
    }

    public class UserLoginIntPk : IdentityUserLogin<long>
    {
    }

    public class RoleIntPk : IdentityRole<long, UserRoleIntPk>
    {
        public RoleIntPk() { }
        public RoleIntPk(string name) { Name = name; }
    }

    public class UserStoreIntPk : UserStore<ApplicationUser, RoleIntPk, long,
       UserLoginIntPk, UserRoleIntPk, UserClaimIntPk>
    {
        public UserStoreIntPk(ApplicationDbContext context)
            : base(context)
        {
        }
    }

    public class RoleStoreIntPk : RoleStore<RoleIntPk, long, UserRoleIntPk>
    {
        public RoleStoreIntPk(ApplicationDbContext context)
            : base(context)
        {
        }
    }
}