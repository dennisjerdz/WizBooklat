using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using WizBooklat.Models;

namespace WizBooklat.Controllers
{
    [Authorize(Roles = "Admin")]
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
        
        public ActionResult ActivateAccount(string email)
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
            return RedirectToAction("Accounts");
        }
        
        public ActionResult DisableAccount(string email)
        {
            var user = db.Users.FirstOrDefault(u => u.Email == email);
            if (user != null)
            {
                user.AccountStatus = AccountStatusConstant.DISABLED;
            }
            db.SaveChanges();
            TempData["Message"] = "<strong>Account has been disabled.</strong> We've sent a notification to "+ user.Email +".";
            return RedirectToAction("Accounts");
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