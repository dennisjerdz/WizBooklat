using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using WizBooklat.Models;

namespace WizBooklat.Controllers
{
    public class BookUnitsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: BookUnits
        public ActionResult Index()
        {
            var books = db.Books.Include(b => b.BookTemplate).Include(b => b.Branch);
            return View(books.ToList());
        }

        // GET: BookUnits/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Book book = db.Books.Find(id);
            if (book == null)
            {
                return HttpNotFound();
            }
            return View(book);
        }

        // GET: BookUnits/Create
        public ActionResult Create()
        {
            ViewBag.BookTemplateId = new SelectList(db.BookTemplates, "BookTemplateId", "Title");
            ViewBag.BranchId = new SelectList(db.Branches, "BranchId", "Name");
            return View();
        }

        // POST: BookUnits/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "BookId,InternalCode,BranchId,BookTemplateId,BookStatus")] Book book)
        {
            if (ModelState.IsValid)
            {
                db.Books.Add(book);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.BookTemplateId = new SelectList(db.BookTemplates, "BookTemplateId", "Title", book.BookTemplateId);
            ViewBag.BranchId = new SelectList(db.Branches, "BranchId", "Name", book.BranchId);
            return View(book);
        }

        // GET: BookUnits/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Book book = db.Books.Find(id);
            if (book == null)
            {
                return HttpNotFound();
            }
            ViewBag.BookTemplateId = new SelectList(db.BookTemplates, "BookTemplateId", "Title", book.BookTemplateId);
            ViewBag.BranchId = new SelectList(db.Branches, "BranchId", "Name", book.BranchId);
            return View(book);
        }

        // POST: BookUnits/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "BookId,InternalCode,BranchId,BookTemplateId,BookStatus")] Book book)
        {
            if (ModelState.IsValid)
            {
                db.Entry(book).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.BookTemplateId = new SelectList(db.BookTemplates, "BookTemplateId", "Title", book.BookTemplateId);
            ViewBag.BranchId = new SelectList(db.Branches, "BranchId", "Name", book.BranchId);
            return View(book);
        }

        // GET: BookUnits/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Book book = db.Books.Find(id);
            if (book == null)
            {
                return HttpNotFound();
            }
            return View(book);
        }

        // POST: BookUnits/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Book book = db.Books.Find(id);
            db.Books.Remove(book);
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
