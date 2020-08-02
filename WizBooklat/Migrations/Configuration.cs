namespace WizBooklat.Migrations
{
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.EntityFramework;
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;
    using WizBooklat.Models;

    /* Specs
        WIZBOOKLAT

        Reports
        - choose kind of report, show graphs and tables then print

        Books properties
        - popularity
        - multiple categories
        - author
        - date published
        - review
        - is featured
        - date added

        Book depreciation
        - book can be added
        - once may new version, display old
        - but suggest that there's a new version

        Settings
        - date limit for reserving of books
        - book limit / number of books allowed to reserve
        - email suffix editable by admin '@editable.com'
        - minimum start days of reservation
        - maximum days of reservation

        Roles
        - Admin - manage accounts, set limits, book management
        - Librarian - book management
        - Customer
        - Entrance

        User properties
        - branch id, when adding or returning books, this will be default location

        Web App Book Reservation Process - Online
        - sign up w/ email and student number, login
        - choose book
        - choose date range (reservation minimum start date is 2 days)
        - submit
        - pending points is shown

        Web App Book Reservation Process - Walkin
        - Bring book to librarian
        - Librarian will search book in system
        - click Reserve book for student
        - Librarian will search for student in system
        - submit

        Reservation Limits
        - if book isn't taken on the start date of reservation,
        reservation is cancelled the next day
        - minimum start date for reservation is 2 days, can only be weekdays
        - maximum days of reservation is 14 days
        - Loan periods are 7 and 14 (editable)

        Book return Process
        - librarian will search student
        - click reserved books
        - confirm return
        - fill out form; damaged, description
        - if no damage and returned on-time, points will be approved
        - if book is damaged or late, points will be cancelled

        Book Gallery
        - Can filter according to book properties
        - Can click book and see full details and availability

        Librarian
        - add book, search isbn if wala, manual input
        - location field so this can be transferred within multiple branches

        Upsale
        - Featured books (click featured checkbox), Newly Added Books
        - Book reaver, hall of fame, newly added books

        Points earned when Visiting
        - login with entrance type account
        - open login page
        - if student arrives, guard will tell student to input student number & email, show id
        - points are automatically added

        Other Resources
        - besides books, there are other materials that can be reserved
        - will undergo same process
        - can be found on a separate page

        Perks
        - additional days per book
        - additional limit of books
        - other perks

        Rank Object
        - Add, Edit Rank
        - Each rank has perks (Perks object, duration/ date range)
        - Each rank has rewards (Rewards object, duration/ date range, 1 claim)
        - Bronze 300
        - Silver 500
        - Gold 1000

        Rewards
        - when librarian or admmin adds reward, 
        all existing members within that rank can claim
        - 1 claim, upon claiming, code will be shown

        Earned points
        - 25, return book with no damage and on-time
        - 5, visit per day. If multiple visits within a day, only count 1
        - 25, review and comment

        Book Status
        - Available
        - Available - Damaged (can not be reserved but can be used inside library)
        - Reserved
        - Pulled-out
        - Lost - Reported (1 week to find the book if just misplaced)
        - Lost - Confirmed

        Inquiry Form, no more chat
        - once filledout and submitted, sends email to admin or librarian
        - admin/librarian will reply manually with email
    */

    // Open Library Search: http://openlibrary.org/search.json?title=the+lord+of+the+rings
    // Open Library Get Book: https://openlibrary.org/api/books?bibkeys=ISBN:0451526538&jscmd=data&format=json
    // DB: user / wbadmin@123

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
                    Code = "EMAIL_SUFFIX",
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

            context.SaveChanges();

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
                    AccountStatus = AccountStatusConstant.ACTIVE
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
                    AccountStatus = AccountStatusConstant.ACTIVE
                };
                userManager.Create(user, "wblibrarian@123");
                userManager.AddToRole(user.Id, "Librarian");
            }

            if (!context.Users.Any(u => u.UserName == "test1@mailinator.com"))
            {
                var user = new ApplicationUser
                {
                    FirstName = "Test 1",
                    UserName = "test1@mailinator.com",
                    Email = "test1@mailinator.com",
                    EmailConfirmed = true,
                    AccountStatus = AccountStatusConstant.ACTIVE
                };
                userManager.Create(user, "test1@123");
                userManager.AddToRole(user.Id, "Loaner");
            }

            if (!context.Users.Any(u => u.UserName == "wbloaner1@mailinator.com"))
            {
                var user = new ApplicationUser
                {
                    FirstName = "Loaner T1",
                    UserName = "wbloaner1@mailinator.com",
                    Email = "wbloaner1@mailinator.com",
                    EmailConfirmed = true,
                    AccountStatus = AccountStatusConstant.ACTIVE
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
                    AccountStatus = AccountStatusConstant.ACTIVE
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
                    AccountStatus = AccountStatusConstant.ACTIVE
                };
                userManager.Create(user, "wbentrance@123");
                userManager.AddToRole(user.Id, "Entrance");
            }
        }
    }
}
