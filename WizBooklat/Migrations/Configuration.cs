namespace WizBooklat.Migrations
{
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.EntityFramework;
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;
    using WizBooklat.Models;

    // wb-db.
    // user / wbadmin@123

    internal sealed class Configuration : DbMigrationsConfiguration<WizBooklat.Models.ApplicationDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(WizBooklat.Models.ApplicationDbContext context)
        {
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method 
            //  to avoid creating duplicate seed data.

            if (!context.Branches.Any(b => b.Name == "Mapua Makati"))
            {
                context.Branches.Add(new Models.Branch()
                {
                    Name = "Mapua Makati"
                });
            }

            if (!context.Branches.Any(b => b.Name == "Mapua Intramuros"))
            {
                context.Branches.Add(new Models.Branch()
                {
                    Name = "Mapua Intramuros"
                });
            }

            /* Separator Separator Separator Separator Separator Separator Separator */

            if (!context.Settings.Any(s => s.Code == "EMAIL_SUFFIX"))
            {
                context.Settings.Add(new Models.Setting()
                {
                    Code = "EMAIL_SUFFX",
                    Value = "@mymapua.edu.ph"
                });
            }

            if (!context.Settings.Any(s => s.Code == "NUMBER_OF_ACTIVE_BOOK_LOANS"))
            {
                context.Settings.Add(new Models.Setting()
                {
                    Code = "NUMBER_OF_ACTIVE_BOOK_LOANS",
                    Value = "2"
                });
            }

            /* Separator Separator Separator Separator Separator Separator Separator */

            var userStore = new UserStore<ApplicationUser>(context);
            var userManager = new UserManager<ApplicationUser>(userStore);

            userManager.UserValidator = new UserValidator<ApplicationUser>(userManager)
            {
                AllowOnlyAlphanumericUserNames = false,
            };

            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(new ApplicationDbContext()));

            if (!roleManager.RoleExists("Admin"))
            {
                var role = new IdentityRole();
                role.Name = "Admin";
                roleManager.Create(role);
            }

            if (!roleManager.RoleExists("Librarian"))
            {
                var role = new IdentityRole();
                role.Name = "Librarian";
                roleManager.Create(role);
            }

            if (!roleManager.RoleExists("Loaner"))
            {
                var role = new IdentityRole();
                role.Name = "Loaner";
                roleManager.Create(role);
            }

            if (!roleManager.RoleExists("Entrance"))
            {
                var role = new IdentityRole();
                role.Name = "Entrance";
                roleManager.Create(role);
            }

            if (!context.Users.Any(u => u.UserName == "wbadmin@mailinator.com"))
            {
                var user = new ApplicationUser
                {
                    FirstName = "Administrator T1",
                    UserName = "wbadmin@mailinator.com",
                    Email = "wbadmin@mailinator.com",
                    EmailConfirmed = true,
                };
                userManager.Create(user, "wbadmin@123");
                userManager.AddToRole(user.Id, "Admin");
            }

            if (!context.Users.Any(u => u.UserName == "wblibrarian@mailinator.com"))
            {
                var user = new ApplicationUser
                {
                    FirstName = "Librarian T1",
                    UserName = "wblibrarian@mailinator.com",
                    Email = "wblibrarian@mailinator.com",
                    EmailConfirmed = true,
                };
                userManager.Create(user, "wblibrarian@123");
                userManager.AddToRole(user.Id, "Librarian");
            }

            if (!context.Users.Any(u => u.UserName == "wbloaner1@mailinator.com"))
            {
                var user = new ApplicationUser
                {
                    FirstName = "Loaner T1",
                    UserName = "wbloaner1@mailinator.com",
                    Email = "wbloaner1@mailinator.com",
                    EmailConfirmed = true,
                };
                userManager.Create(user, "wbloaner1@123");
                userManager.AddToRole(user.Id, "Loaner");
            }

            if (!context.Users.Any(u => u.UserName == "wbloaner2@mailinator.com"))
            {
                var user = new ApplicationUser
                {
                    FirstName = "Loaner T2",
                    UserName = "wbloaner2@mailinator.com",
                    Email = "wbloaner2@mailinator.com",
                    EmailConfirmed = true,
                };
                userManager.Create(user, "wbloaner2@123");
                userManager.AddToRole(user.Id, "Loaner");
            }

            if (!context.Users.Any(u => u.UserName == "wbentrance@mailinator.com"))
            {
                var user = new ApplicationUser
                {
                    FirstName = "Entrance T1",
                    UserName = "wbentrance@mailinator.com",
                    Email = "wbentrance@mailinator.com",
                    EmailConfirmed = true,
                };
                userManager.Create(user, "wbentrance@123");
                userManager.AddToRole(user.Id, "Entrance");
            }
        }
    }
}
