﻿using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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
        public ApplicationUser() : base()
        {
            Elements = new List<Element>();            
        }
        [MaxLength(255)]
        [Display(Name = "الإسم")]
        public string Name { get; set; }
        [Display(Name = "الصورة")]
        public string Image { get; set; }
        [Display(Name = "الجهة")]
        public int? EntityID { get; set; }
        public Entity Entity { get; set; }
        public virtual List<Element> Elements { get; set; }
        public bool IsDeleted { get; set; }
        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);

            if (!string.IsNullOrWhiteSpace(this.Name))
                userIdentity.AddClaim(new Claim("Name", this.Name));
            if (!string.IsNullOrWhiteSpace(this.Image))
                userIdentity.AddClaim(new Claim("Image", this.Image));

            return userIdentity;
        }
    }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
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
        public DbSet<ElementValue> ElementValues { get; set; }      
        public DbSet<Element> Elements { get; set; }
        public DbSet<Entity> Entities { get; set; }
        public DbSet<Equation> Equations { get; set; }
        public DbSet<GeoArea> GeoAreas { get; set; }        
        public DbSet<SystemLog> SystemLogs { get; set; }
        public DbSet<UpdateLog> UpdateLogs { get; set; }        
        public DbSet<EquationElement> EquationElements { get; set; }
        public DbSet<EquationYear> EquationYears { get; set; }
        public DbSet<CalculatedValue> CalculatedValues { get; set; }
        public DbSet<ElementYearValue> ElementYearValues { get; set; }
        public DbSet<OfficerLog> OfficerLogs { get; set; }
        public DbSet<IndicatorLimit> IndicatorLimits { get; set; }
    }
}