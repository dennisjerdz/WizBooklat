using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using WizBooklat.Models;
using ViewModels = WizBooklat.Models.ViewModels;

namespace WizBooklat.Controllers
{
    [Authorize(Roles = "Admin,Librarian")]
    public class AdminController : Controller
    {
        #region Account Manager
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;

        private ApplicationDbContext db = new ApplicationDbContext();

        public AdminController()
        {
        }

        public AdminController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
        {
            UserManager = userManager;
            SignInManager = signInManager;
        }

        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set
            {
                _signInManager = value;
            }
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }
        #endregion

        public ActionResult Reports()
        {
            return View();
        }

        [HttpPost]
        public async Task<PartialViewResult> GetReport(string reportType)
        {
            ViewModels.ReportViewModel rvm = new ViewModels.ReportViewModel();
            rvm.ReportType = reportType;

            switch (reportType)
            {
                case "Top Users":
                    rvm.Users = await db.Users.Include(u=>u.PointHistory).Where(u=>u.AccountType == AccountTypeConstant.LOANER).ToListAsync();
                    break;
                case "Top Books":
                    rvm.TopBooks = await db.Loans.Include(b => b.Book).GroupBy(b => b.Book.BookTemplate)
                        .Select(b => new ViewModels.ReportViewModel.BookCount { Name = b.Key.Title, Count = b.Count() }).ToListAsync();
                    break;
                case "Genre Tally":
                    rvm.Genres = await db.Genres.Include(g=>g.BookGenres).OrderByDescending(g=>g.BookGenres.Count()).ToListAsync();
                    break;
                case "Author Tally":
                    rvm.Authors = await db.Authors.Include(a=>a.BookAuthors).OrderByDescending(a=>a.BookAuthors.Count()).ToListAsync();
                    break;
                case "Loans Past 30 days":

                    DateTime today = DateTime.UtcNow.AddHours(8).AddDays(1).Date;
                    DateTime daysAgo = DateTime.UtcNow.AddHours(8).AddDays(-29).Date;

                    rvm.TransactionsPast30Days = new List<WizBooklat.Models.ViewModels.ReportViewModel.TransactionsDatetimeCountModel>();
                    for (var dt = daysAgo; dt <= today; dt = dt.AddDays(1))
                    {
                        rvm.TransactionsPast30Days.Add(new WizBooklat.Models.ViewModels.ReportViewModel.TransactionsDatetimeCountModel { Date = dt, Count = 0 });
                    }

                    var transactionsPast30DaysDB = db.Loans
                        .Where(t =>
                            //t.IsCancelled == false &&
                            (t.DateCreated <= today && t.DateCreated >= daysAgo)
                        ).ToList()
                        .GroupBy(t => t.DateCreated.Date)
                        .ToList();

                    foreach (var item in transactionsPast30DaysDB)
                    {
                        if (rvm.TransactionsPast30Days.FirstOrDefault(x => x.Date == item.Key) != null)
                        {
                            rvm.TransactionsPast30Days.FirstOrDefault(x => x.Date == item.Key).Count = item.Count();
                        }
                    }

                    break;
                case "Loans This Year":

                    int year = DateTime.UtcNow.AddHours(8).Year;
                    rvm.TransactionsThisYear = new List<WizBooklat.Models.ViewModels.ReportViewModel.TransactionsDatetimeCountModel>();

                    for (int month = 1; month <= 12; month++)
                    {
                        var loansThisMonth = await db.Loans.Where(l => l.DateCreated.Year == year && l.DateCreated.Month == month).ToListAsync() ;
                        rvm.TransactionsThisYear.Add(new ViewModels.ReportViewModel.TransactionsDatetimeCountModel
                        {
                            Date = DateTime.Parse(month.ToString() + "/1/" + year.ToString()),
                            Count = loansThisMonth.Count()
                        });
                    };
                    break;
            }

            return PartialView("_ReportBody",rvm);
        }

        public ActionResult CancelExpiredLoan(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Loan loan = db.Loans.Find(id);
            if (loan == null)
            {
                return HttpNotFound();
            }

            loan.ReturnDate = DateTime.UtcNow.AddHours(8);
            loan.IsCancelled = true;
            loan.Book.BookStatus = BookStatusConstant.AVAILABLE;
            db.SaveChanges();
            TempData["Message"] = "<strong>Reservation has been cancelled successfully.</strong>";
            return RedirectToAction("ViewLoans", new { email = loan.User.Email });
        }

        public ActionResult ProcessClaim(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Loan loan = db.Loans.Find(id);
            if (loan == null)
            {
                return HttpNotFound();
            }
            return View(loan);
        }

        [HttpPost]
        public ActionResult ProcessClaim(ViewModels.ClaimBookModel cbm)
        {
            if (cbm.LoanId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Loan loan = db.Loans.Find(cbm.LoanId);
            if (loan == null)
            {
                TempData["Error"] = "1";
                TempData["Message"] = "<strong>Failed to initiate return.</strong> Loan ID does not exist.";
                return RedirectToAction("ActiveLoans", new { });
            }

            if (loan.ReturnDate != null)
            {
                TempData["Error"] = "1";
                TempData["Message"] = "<strong>Failed to initiate return.</strong> This reservation is already completed, the book has been returned last " + loan.ReturnDate.Value.ToString("MM-dd-yyyy hh:mm tt");
                return RedirectToAction("ViewLoans", new { email = loan.User.Email });
            }

            loan.PickUpDate = DateTime.UtcNow.AddHours(8);
            
            db.SaveChanges();

            TempData["Message"] = "<strong>Successfully added Claim date / Pick up date.</strong> The book, " + loan.Book.BookTemplate.Title + " with ID " + loan.BookId + ", has been picked up.";
            try
            {
                Task.Run(() =>
                {
                    new SMSController().SendSMS(loan.UserId, "Hello, this is to confirm that you've claimed the book; " + loan.Book.BookTemplate.Title + " on " + cbm.ClaimDate.ToString("MM-dd-yyyy") + " from Loan ID; " + loan.LoanId + ". Please don't forget to return the book ON or BEFORE its due date; " + loan.EndDate.ToString("MM-dd-yyyy"));
                });
            }
            catch (Exception e)
            {
                Trace.TraceInformation("SMS Send Failed for " + loan.Book.BookTemplate.Title + ", Loan ID:" + loan.LoanId + ": " + e.Message);
            }
            return RedirectToAction("ViewLoans", new { email = loan.User.Email });
        }

        public ActionResult ProcessReturn(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Loan loan = db.Loans.Find(id);
            if (loan == null)
            {
                return HttpNotFound();
            }
            return View(loan);
        }

        [HttpPost]
        public ActionResult ProcessReturn(ViewModels.ReturnBookModel rbm)
        {
            if (rbm.LoanId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Loan loan = db.Loans.Find(rbm.LoanId);
            if (loan == null)
            {
                TempData["Error"] = "1";
                TempData["Message"] = "<strong>Failed to initiate return.</strong> Loan ID does not exist.";
                return RedirectToAction("ActiveLoans", new { });
            }

            if (loan.ReturnDate != null)
            {
                TempData["Error"] = "1";
                TempData["Message"] = "<strong>Failed to initiate return.</strong> This reservation is already completed, the book has been returned last "+ loan.ReturnDate.Value.ToString("MM-dd-yyyy hh:mm tt");
                return RedirectToAction("ViewLoans", new { email = loan.User.Email });
            }
            
            loan.ReturnDate = DateTime.UtcNow.AddHours(8);
            loan.IsDamaged = rbm.IsDamaged;
            loan.IsSevere = rbm.IsSevere;
            loan.DamageDescription = rbm.DamageDescription;

            if (rbm.IsDamaged)
            {
                if (rbm.IsSevere)
                {
                    loan.Book.BookStatus = BookStatusConstant.DAMAGED_AVAILABLE;
                    TempData["Message"] = "<strong>Successfully returned book.</strong> The book, " + loan.Book.BookTemplate.Title + " with ID " + loan.BookId + ", has been set to Available - Damaged.";
                }
                else
                {
                    loan.Book.BookStatus = BookStatusConstant.AVAILABLE;
                    TempData["Message"] = "<strong>Successfully returned book.</strong> The book, " + loan.Book.BookTemplate.Title + " with ID " + loan.BookId + ", has been set to Available.";
                }
            }
            else
            {
                loan.Book.BookStatus = BookStatusConstant.AVAILABLE;
                TempData["Message"] = "<strong>Successfully returned book.</strong> The book, " + loan.Book.BookTemplate.Title + " with ID " + loan.BookId + ", has been set to Available.";
            }
            
            db.SaveChanges();

            try
            {

                Task.Run(() =>
                {
                    new SMSController().SendSMS(loan.UserId, "Hello, this is to confirm that you've returned the book; " + loan.Book.BookTemplate.Title + " on " + loan.ReturnDate.Value.ToString("MM-dd-yyyy hh:mm tt") + " from Loan ID; " + loan.LoanId + ". ");
                });
            }
            catch (Exception e)
            {
                Trace.TraceInformation("SMS Send Failed for " + loan.Book.BookTemplate.Title + ", Loan ID:" + loan.LoanId + ": " + e.Message);
            }

            if (loan.IsDamaged == false && loan.ReturnDate.Value.Date <= loan.EndDate)
            {
                try
                {
                    PointHistory newPoints = new PointHistory
                    {
                        DateCreated = DateTime.UtcNow.AddHours(8),
                        Description = "Points earned from returning the book; " + loan.Book.BookTemplate.Title + " with Loan ID " + loan.LoanId + " on time.",
                        Type = PointTypeConstant.ADD,
                        Points = 15,
                        UserId = loan.UserId
                    };

                    db.PointHistories.Add(newPoints);
                    db.SaveChanges();

                    int totalAddPoints = db.PointHistories.Where(p => p.UserId == loan.UserId && p.Type == PointTypeConstant.ADD).Sum(p => p.Points);
                    int totalMinusPoints = (db.PointHistories.Where(p => p.UserId == loan.UserId && p.Type == PointTypeConstant.MINUS).Count() == 0) ? 0 : db.PointHistories.Where(p => p.UserId == loan.UserId && p.Type == PointTypeConstant.MINUS).Sum(p => p.Points);
                    int totalPoints = totalAddPoints - totalMinusPoints;

                    Task.Run(() =>
                    {
                        new SMSController().SendSMS(loan.UserId, "Thank you for returning "+loan.Book.BookTemplate.Title+" on time and in good condition! You've earned an additional "+newPoints.Points+". Your total points is now "+totalPoints+".");
                    });
                }
                catch (Exception e)
                {
                    Trace.TraceInformation("SMS Send Failed for " + loan.Book.BookTemplate.Title + ", Loan ID:" + loan.LoanId + ": " + e.Message);
                }
            }
            
            return RedirectToAction("ViewLoans", new { email = loan.User.Email });
        }

        public ActionResult ViewLoans(string email)
        {
            if (TempData["Error"] != null)
            {
                ViewBag.Error = TempData["Error"];
            }
            if (TempData["Message"] != null)
            {
                ViewBag.Message = TempData["Message"];
            }

            var user = db.Users.FirstOrDefault(u => u.Email == email);
            if (user != null)
            {
                ViewBag.User = user;
                return View(db.Loans.Where(l=>l.UserId == user.Id).ToList());
            }
            else
            {
                TempData["Error"] = "1";
                TempData["Message"] = "<strong>Failed to open Student Book Loans.</strong> Username not found.";
                return RedirectToAction("Students");
            }
        }

        public ActionResult ActiveLoans()
        {
            if (TempData["Error"] != null)
            {
                ViewBag.Error = TempData["Error"];
            }
            if (TempData["Message"] != null)
            {
                ViewBag.Message = TempData["Message"];
            }

            return View(db.Loans.Where(l => l.ReturnDate == null).ToList());
        }

        public ActionResult CompletedLoans()
        {
            if (TempData["Error"] != null)
            {
                ViewBag.Error = TempData["Error"];
            }
            if (TempData["Message"] != null)
            {
                ViewBag.Message = TempData["Message"];
            }

            return View(db.Loans.Where(l => l.ReturnDate != null).ToList());
        }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Accounts()
        {
            if (TempData["Error"] != null)
            {
                ViewBag.Error = TempData["Error"];
            }
            if (TempData["Message"] != null)
            {
                ViewBag.Message = TempData["Message"];
            }
            
            var users = db.Users.ToList();
            return View(users);
        }

        public ActionResult Students()
        {
            if (TempData["Error"] != null)
            {
                ViewBag.Error = TempData["Error"];
            }
            if (TempData["Message"] != null)
            {
                ViewBag.Message = TempData["Message"];
            }

            ViewBag.ranks = db.Ranks.OrderBy(r=>r.Points).ToList();
            var users = db.Users.Include(u=>u.Loans).Include(u=>u.PointHistory).Where(u=>u.AccountType == AccountTypeConstant.LOANER).ToList();
            return View(users);
        }
        
        public ActionResult ActivateAccount(string email, string redirect = "Accounts")
        {
            var user = db.Users.FirstOrDefault(u => u.Email == email);
            if (user != null)
            {
                user.AccountStatus = AccountStatusConstant.ACTIVE;
            }
            db.SaveChanges();

            string body = "Hello " + user.FirstName + ", your WizBooklat account has been activated. Please login <a href='http://wizbooklat.azurewebsites.net/Account/Login/'>here.</a>";
            string fromName = "WizBooklat Registration";
            string subject = "WizBooklat Account has been Activated!"; //+ DateTime.UtcNow.AddHours(8).ToString("MM-dd-yy");

            try
            {
                WBHelper.SendEmail(fromName, subject, body, user.Email, user.FirstName);
            }
            catch (Exception e)
            {
                return Content(e.Message);
            }

            TempData["Message"] = "<strong>Account has been activated.</strong> We've sent a notification to "+user.Email+".";
            return RedirectToAction(redirect);
        }
        
        public ActionResult DisableAccount(string email, string redirect = "Accounts")
        {
            var user = db.Users.FirstOrDefault(u => u.Email == email);
            if (user != null)
            {
                user.AccountStatus = AccountStatusConstant.DISABLED;
            }
            db.SaveChanges();
            TempData["Message"] = "<strong>Account has been disabled.</strong> We've sent a notification to "+ user.Email +".";
            return RedirectToAction(redirect);
        }

        public async Task<ActionResult> DeleteAccount(string email, string redirect = "Accounts")
        {
            var user = await UserManager.FindByEmailAsync(email);

            if (user != null)
            {
                string userId = user.Id;
                var logins = user.Logins;
                var rolesForUser = await UserManager.GetRolesAsync(userId);

                using (var transaction = db.Database.BeginTransaction())
                {
                    foreach (var login in logins.ToList())
                    {
                        await UserManager.RemoveLoginAsync(login.UserId, new UserLoginInfo(login.LoginProvider, login.ProviderKey));
                    }

                    if (rolesForUser.Count() > 0)
                    {
                        foreach (var item in rolesForUser.ToList())
                        {
                            // item should be the name of the role
                            var result = await UserManager.RemoveFromRoleAsync(user.Id, item);
                        }
                    }

                    await UserManager.DeleteAsync(user);
                    transaction.Commit();
                }

                db.SaveChanges();
                TempData["Message"] = "<strong>Account has been deleted successfully.</strong>";
                return RedirectToAction(redirect);
            }

            TempData["Error"] = "1";
            TempData["Message"] = "<strong>Failed to delete account; </strong> User not found.";
            return RedirectToAction(redirect);
        }


        public ActionResult AddAccount()
        {
            RegisterViewModel rvm = new RegisterViewModel();

            if (TempData["Error"] != null)
            {
                ViewBag.Error = TempData["Error"];
            }
            if (TempData["Message"] != null)
            {
                ViewBag.Message = TempData["Message"];
            }

            ViewBag.Branches = db.Branches.ToList();

            return View(rvm);
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> AddAccount(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser
                {
                    FirstName = model.FirstName,
                    MiddleName = model.MiddleName,
                    LastName = model.LastName,
                    MobileNumber = model.MobileNumber,
                    UserName = model.Email,
                    Email = model.Email,
                    BranchId = model.BranchId,
                    EmailConfirmed = true,
                    AccountType = model.AccountType,
                    AccountStatus = AccountStatusConstant.ACTIVE
                };

                var result = await UserManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    switch (model.AccountType)
                    {
                        case AccountTypeConstant.ADMIN:
                            UserManager.AddToRole(user.Id, "Admin");
                            break;
                        case AccountTypeConstant.LIBRARIAN:
                            UserManager.AddToRole(user.Id, "Librarian");
                            break;
                        case AccountTypeConstant.LOANER:
                            UserManager.AddToRole(user.Id, "Loaner");
                            break;
                        case AccountTypeConstant.ENTRANCE:
                            UserManager.AddToRole(user.Id, "Entrance");
                            break;
                    }
                    
                    await db.SaveChangesAsync();

                    TempData["Message"] = "<strong>Account has been created.</strong>.";
                    return RedirectToAction("Accounts", "Admin");
                }
                AddErrors(result);
            }

            ViewBag.Branches = db.Branches.ToList();
            // If we got this far, something failed, redisplay form
            return View(model);
        }
        
        public ActionResult EditAccount(string email)
        {
            if (TempData["Error"] != null)
            {
                ViewBag.Error = TempData["Error"];
            }
            if (TempData["Message"] != null)
            {
                ViewBag.Message = TempData["Message"];
            }

            var user = db.Users.FirstOrDefault(u => u.UserName == email);

            if (user == null)
            {
                TempData["Error"] = "1";
                TempData["Message"] = "<strong>Account not found.</strong>";
                return RedirectToAction("Accounts");
            }

            var editAccountModel = new EditAccountViewModel
            {
                BranchId = user.BranchId,
                Email = user.Email,
                FirstName = user.FirstName,
                MiddleName = user.MiddleName,
                LastName = user.LastName,
                AccountType = user.AccountType
            };

            ViewBag.Branches = db.Branches.ToList();
            return View(editAccountModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditAccount(EditAccountViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = db.Users.FirstOrDefault(u => u.Email == model.Email);

                if (user == null)
                {
                    TempData["Error"] = "1";
                    TempData["Message"] = "<strong>Account not found.</strong>";
                    return RedirectToAction("Accounts");
                }
                else
                {
                    user.FirstName = model.FirstName;
                    user.MiddleName = model.MiddleName;
                    user.LastName = model.LastName;
                    user.BranchId = model.BranchId;

                    db.SaveChanges();
                    TempData["Message"] = "<strong>Account has been updated successfully.</strong>";
                    return RedirectToAction("Accounts");
                }
            }

            return View(model);
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }
    }
}