using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace Marsad.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit https://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {
        public int? UserGroupID { get; set; }
        public UserGroup UserGroup { get; set; }
        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }
    }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
        }

        public DbSet<Bundle> Bundles { get; set; }
        public DbSet<DataSourceGroup> DataSourceGroups { get; set; }
        public DbSet<DataSourceType> DataSourceTypes { get; set; }
        public DbSet<Period> Periods { get; set; }
        public DbSet<IndicatorGroup> IndicatorGroups { get; set; }
        public DbSet<IndicatorType> IndicatorTypes { get; set; }
        public DbSet<Indicator> Indicators { get; set; }
        public DbSet<DataSource> DataSources { get; set; }
        public DbSet<Case> Cases { get; set; }
        public DbSet<CaseYear> CaseYears { get; set; }
        public DbSet<CaseYearIndicator> CaseYearIndicators { get; set; }



        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }

        public System.Data.Entity.DbSet<Marsad.Models.Element> Elements { get; set; }

        public System.Data.Entity.DbSet<Marsad.Models.Entity> Entities { get; set; }

        public System.Data.Entity.DbSet<Marsad.Models.Equation> Equations { get; set; }

        public System.Data.Entity.DbSet<Marsad.Models.GeoArea> GeoAreas { get; set; }

        public System.Data.Entity.DbSet<Marsad.Models.GeoAreaBundle> GeoAreaBundles { get; set; }

        public System.Data.Entity.DbSet<Marsad.Models.UserGroup> UserGroups { get; set; }
        public System.Data.Entity.DbSet<Marsad.Models.EquationElement> EquationElements{ get; set; }
        public System.Data.Entity.DbSet<Marsad.Models.MyClaim> MyClaims { get; set; }

    }
}