using System.Collections.Generic;
using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace WizBooklat.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit https://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string StudentNumber { get; set; } 
        public string MobileNumber { get; set; }
        public string MobileNumberCode { get; set; }

        public Int16 AccountType { get; set; }
        public Int16 AccountStatus { get; set; }

        public int? BranchId { get; set; }
        [ForeignKey("BranchId")]
        public virtual Branch Branch { get; set; }

        public List<Loan> Loans { get; set; }

        public List<Review> Reviews { get; set; }

        public List<Visit> Visits { get; set; }

        public List<PointHistory> PointHistory { get; set; }

        public List<Claim> RewardClaims { get; set; }

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
        { }

        public DbSet<Branch> Branches { get; set; }
        public DbSet<BookTemplate> BookTemplates { get; set; }
        public DbSet<Book> Books { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<Loan> Loans { get; set; }
        public DbSet<Genre> Genres { get; set; }
        public DbSet<BookGenre> BookGenres { get; set; }
        public DbSet<Author> Authors { get; set; }
        public DbSet<Visit> Visits { get; set; }
        public DbSet<PointHistory> PointHistories { get; set; }
        public DbSet<Rank> Ranks { get; set; }
        public DbSet<Reward> Rewards { get; set; }
        public DbSet<Claim> Claims { get; set; }
        public DbSet<Setting> Settings { get; set; }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }
    }
}