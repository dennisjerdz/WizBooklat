using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WizBooklat.Models;

namespace WizBooklat.Controllers
{
    [Authorize(Roles = "Entrance")]
    public class EntranceController : Controller
    {
        // GET: Entrance
        private ApplicationDbContext db = new ApplicationDbContext();

        public ActionResult Index()
        {
            if (TempData["Error"] != null)
            {
                ViewBag.Error = TempData["Error"];
            }
            if (TempData["Message"] != null)
            {
                ViewBag.Message = TempData["Message"];
            }

            return View();
        }

        [HttpPost]
        public ActionResult Index(string studentNumber)
        {
            var user = db.Users.Include(u=>u.PointHistory).FirstOrDefault(u => u.StudentNumber == studentNumber);

            if (user != null)
            {
                DateTime today = DateTime.UtcNow.AddHours(8).Date;

                if (!user.PointHistory.Any(p=>p.Description == "Visit" && p.DateCreated.Date == today))
                {
                    db.PointHistories.Add(new PointHistory
                    {
                        DateCreated = DateTime.UtcNow.AddHours(8),
                        Description = "Visit",
                        Points = 15,
                        Type = PointTypeConstant.ADD,
                        UserId = user.Id
                    });

                    db.SaveChanges();
                }
                
                TempData["Message"] = "<strong>Welcome "+user.FirstName+"!</strong> You've earned 15 points today.";
            }
            else
            {
                TempData["Error"] = "1";
                TempData["Message"] = "Student number not found.";
            }

            return RedirectToAction("Index");
        }
    }
}