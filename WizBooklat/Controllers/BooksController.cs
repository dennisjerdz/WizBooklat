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
    public class BooksController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Books
        public ActionResult Index()
        {
            return View(db.BookTemplates.ToList());
        }

        // GET: Books/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BookTemplate bookTemplate = db.BookTemplates.Find(id);
            if (bookTemplate == null)
            {
                return HttpNotFound();
            }
            return View(bookTemplate);
        }

        // GET: Books/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Books/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "BookTemplateId,Title,Description,ImageLocation,Type,LoanPeriod,ISBN,OLKey,PublishDate,InitialQuantity,ActualQuantity,Genres,Authors,IsFeatured,DateCreated")] BookTemplate bookTemplate)
        {
            if (ModelState.IsValid)
            {
                db.BookTemplates.Add(bookTemplate);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(bookTemplate);
        }

        // GET: Books/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BookTemplate bookTemplate = db.BookTemplates.Find(id);
            if (bookTemplate == null)
            {
                return HttpNotFound();
            }
            return View(bookTemplate);
        }

        // POST: Books/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "BookTemplateId,Title,Description,ImageLocation,Type,LoanPeriod,ISBN,OLKey,PublishDate,InitialQuantity,ActualQuantity,Genres,Authors,IsFeatured,DateCreated")] BookTemplate bookTemplate)
        {
            if (ModelState.IsValid)
            {
                db.Entry(bookTemplate).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(bookTemplate);
        }

        // GET: Books/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BookTemplate bookTemplate = db.BookTemplates.Find(id);
            if (bookTemplate == null)
            {
                return HttpNotFound();
            }
            return View(bookTemplate);
        }

        // POST: Books/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            BookTemplate bookTemplate = db.BookTemplates.Find(id);
            db.BookTemplates.Remove(bookTemplate);
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
