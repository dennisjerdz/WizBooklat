using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using WizBooklat.Models;

namespace WizBooklat.Controllers
{
    public class LoansController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        [HttpPost]
        public ActionResult AddReview(int LoanId, int Review, string ReviewDescription)
        {
            Loan loan = db.Loans.Find(LoanId);
            
            if (loan == null)
            {
                TempData["Error"] = "1";
                TempData["Message"] = "<strong>Failed to submit review.</strong> Loan ID not found.";
                return RedirectToAction("MyLoans");
            }

            loan.Review = (short)Review;
            loan.ReviewDescription = ReviewDescription;
            
            string userId = loan.UserId;
            db.PointHistories.Add(new PointHistory
            {
                DateCreated = DateTime.UtcNow.AddHours(8),
                Points = 15,
                Type = PointTypeConstant.ADD,
                UserId = userId
            });

            db.SaveChanges();

            BookTemplate bt = db.BookTemplates.Find(loan.Book.BookTemplateId);
            double? average = bt.Stocks.Average(s => s.LoanHistory.Average(h => h.Review));

            if (average != null)
            {
                bt.ReviewAverage = (float)Math.Round(average.Value,2);
                db.SaveChanges();
            }

            #region Send SMS
            try
            {
                var user =
                Task.Run(() =>
                {
                    new SMSController().SendSMS(userId, "Hello, thank you for reviewing " + loan.Book.BookTemplate.Title + ". You have earned 15 points!");
                });
            }
            catch (Exception e)
            {
                Trace.TraceInformation("SMS Send Failed for " + loan.Book.BookTemplate.Title + ", Loan ID:" + loan.LoanId + ": " + e.Message);
            }
            #endregion
            
            TempData["Message"] = "<strong>Successfully submitted review.</strong> You have earned 15 points.";
            return RedirectToAction("MyLoans");
        }

        public ActionResult AddReview(int? id)
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

        public ActionResult CancelLoan(int? id)
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
            return RedirectToAction("MyLoans");
        }

        // GET: Loans
        public ActionResult Index()
        {
            var loans = db.Loans.Include(l => l.Book).Include(l => l.User);
            return View(loans.ToList());
        }

        [Authorize]
        public ActionResult MyLoans()
        {
            if (TempData["Error"] != null)
            {
                ViewBag.Error = TempData["Error"];
            }
            if (TempData["Message"] != null)
            {
                ViewBag.Message = TempData["Message"];
            }

            string userId = User.Identity.GetUserId();
            return View(db.Loans.Where(l => l.UserId == userId).ToList());
        }

        // GET: Loans/Details/5
        public ActionResult Details(int? id)
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

        // GET: Loans/Create
        public ActionResult Create()
        {
            ViewBag.BookId = new SelectList(db.Books, "BookId", "InternalCode");
            ViewBag.UserId = new SelectList(db.Users, "Id", "FirstName");
            return View();
        }

        // POST: Loans/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "LoanId,UserId,BookId,StartDate,EndDate,ReturnDate,IsLate,IsDamaged,LostReported,LostConfirmed,DateCreated")] Loan loan)
        {
            if (ModelState.IsValid)
            {
                db.Loans.Add(loan);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.BookId = new SelectList(db.Books, "BookId", "InternalCode", loan.BookId);
            ViewBag.UserId = new SelectList(db.Users, "Id", "FirstName", loan.UserId);
            return View(loan);
        }

        // GET: Loans/Edit/5
        public ActionResult Edit(int? id)
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
            ViewBag.BookId = new SelectList(db.Books, "BookId", "InternalCode", loan.BookId);
            ViewBag.UserId = new SelectList(db.Users, "Id", "FirstName", loan.UserId);
            return View(loan);
        }

        // POST: Loans/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "LoanId,UserId,BookId,StartDate,EndDate,ReturnDate,IsLate,IsDamaged,LostReported,LostConfirmed,DateCreated")] Loan loan)
        {
            if (ModelState.IsValid)
            {
                db.Entry(loan).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.BookId = new SelectList(db.Books, "BookId", "InternalCode", loan.BookId);
            ViewBag.UserId = new SelectList(db.Users, "Id", "FirstName", loan.UserId);
            return View(loan);
        }

        // GET: Loans/Delete/5
        public ActionResult Delete(int? id)
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

        // POST: Loans/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Loan loan = db.Loans.Find(id);
            db.Loans.Remove(loan);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
