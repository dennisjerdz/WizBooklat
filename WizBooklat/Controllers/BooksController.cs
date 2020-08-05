using Microsoft.AspNet.Identity;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using WizBooklat.Models;
using ViewModels = WizBooklat.Models.ViewModels;

namespace WizBooklat.Controllers
{
    public class BooksController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        [Route("Books/Gallery/")]
        public async Task<ActionResult> Gallery()
        {
            ViewModels.InitialGalleryViewModel ig = new ViewModels.InitialGalleryViewModel();

            ig.Featured = await db.BookTemplates.Where(b => b.IsFeatured == true).OrderByDescending(b=>b.DateCreated).Take(10).ToListAsync();
            ig.NewArrival = await db.BookTemplates.OrderByDescending(b => b.DateCreated).Take(10).ToListAsync();

            var topGenre = await db.Genres.Include(b=>b.BookGenres).OrderByDescending(b => b.BookGenres.Count()).FirstOrDefaultAsync();

            if (topGenre != null)
            {
                ViewBag.TopGenre = topGenre.Name;
                int[] books = topGenre.BookGenres.Select(b=>b.BookTemplateId).Take(10).ToArray();
                ig.TopGenre = await db.BookTemplates.Where(b => books.Contains(b.BookTemplateId)).ToListAsync();
            }

            List<Genre> genreOptions = db.Genres.ToList();
            ViewBag.GenreOptions = genreOptions;
            return View(ig);
        }

        [Route("Books/Gallery/Find/")]
        public async Task<ActionResult> Gallery(string isbn, string title, int[] genreId)
        {
            List<Genre> genreOptions = db.Genres.ToList();
            ViewBag.GenreOptions = genreOptions;

            var books = await db.BookTemplates.Where(b => b.ISBN.StartsWith(isbn) && b.Title.Contains(title) && b.BookGenres.Select(g=>g.GenreId).Contains(genreId.FirstOrDefault())).ToListAsync();

            return View("GalleryFind", books);
        }

        // GET: Books
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
        public ActionResult Create([Bind(Include = "BookTemplateId,Title,Description,ImageLocation,LoanPeriod,ISBN,OLKey,PublishYear,InitialQuantity,Genres,Authors")] BookTemplate bookTemplate)
        {
            int? branchId = null;
            ApplicationUser currentUser;

            bookTemplate.IsFeatured = false;
            bookTemplate.ActualQuantity = bookTemplate.InitialQuantity;
            bookTemplate.Type = WizBooklat.Models.BookTypeConstant.Book;
            bookTemplate.DateCreated = DateTime.UtcNow.AddHours(8);

            string[] genres = { "" };
            string[] authors = { "" };

            if (bookTemplate.Genres != null)
            {
                genres = bookTemplate.Genres.Split(',');
            }

            if (bookTemplate.Authors != null)
            {
                authors = bookTemplate.Authors.Split(',');
            }

            if (ModelState.IsValid)
            {
                db.BookTemplates.Add(bookTemplate);
                db.SaveChanges();
                
                List<Genre> genresToAdd = new List<Genre>();
                List<BookGenre> genresToSave = new List<BookGenre>();
                
                List<Author> authorsToAdd = new List<Author>();
                List<BookAuthor> authorsToSave = new List<BookAuthor>();

                #region Genres
                foreach (string genre in genres.Where(g=>g!=""))
                {
                    Genre findGenre = db.Genres.FirstOrDefault(g => g.Name == genre);

                    if (findGenre != null)
                    {
                        genresToSave.Add(new BookGenre
                        {
                            BookTemplateId = bookTemplate.BookTemplateId,
                            GenreId = findGenre.GenreId,
                            DateCreated = DateTime.UtcNow.AddHours(8)
                        });
                    }
                    else
                    {
                        Genre newGenre = new Genre
                        {
                            DateCreated = DateTime.UtcNow.AddHours(8),
                            Name = genre
                        };

                        genresToAdd.Add(newGenre);
                    }
                }

                db.Genres.AddRange(genresToAdd);
                db.SaveChanges();

                foreach(Genre newGenre in genresToAdd)
                {
                    genresToSave.Add(new BookGenre
                    {
                        BookTemplateId = bookTemplate.BookTemplateId,
                        GenreId = newGenre.GenreId,
                        DateCreated = DateTime.UtcNow.AddHours(8)
                    });
                }

                db.BookGenres.AddRange(genresToSave);
                db.SaveChanges();
                #endregion

                #region Authors
                foreach (string author in authors.Where(a => a != ""))
                {
                    Author findAuthor = db.Authors.FirstOrDefault(a => a.Name == author);

                    if (findAuthor != null)
                    {
                        authorsToSave.Add(new BookAuthor
                        {
                            BookTemplateId = bookTemplate.BookTemplateId,
                            AuthorId = findAuthor.AuthorId,
                            DateCreated = DateTime.UtcNow.AddHours(8)
                        });
                    }
                    else
                    {
                        Author newAuthor = new Author
                        {
                            DateCreated = DateTime.UtcNow.AddHours(8),
                            Name = author
                        };

                        authorsToAdd.Add(newAuthor);
                    }
                }

                db.Authors.AddRange(authorsToAdd);
                db.SaveChanges();

                foreach (Author newAuthor in authorsToAdd)
                {
                    authorsToSave.Add(new BookAuthor
                    {
                        BookTemplateId = bookTemplate.BookTemplateId,
                        AuthorId = newAuthor.AuthorId,
                        DateCreated = DateTime.UtcNow.AddHours(8)
                    });
                }

                db.BookAuthors.AddRange(authorsToSave);
                db.SaveChanges();
                #endregion

                string userId = User.Identity.GetUserId();
                if (!string.IsNullOrEmpty(userId))
                {
                    currentUser = db.Users.FirstOrDefault(u => u.Id == userId);

                    if (currentUser != null)
                    {
                        branchId = currentUser.BranchId;
                    }
                }

                List<Book> bookUnits = new List<Book>();
                for (int count = 0; count < bookTemplate.InitialQuantity; count++)
                {
                    var bookToAdd = new Book {
                        BookStatus = BookStatusConstant.AVAILABLE,
                        BookTemplateId = bookTemplate.BookTemplateId,
                        BranchId = branchId
                    };

                    bookUnits.Add(bookToAdd);
                }

                db.Books.AddRange(bookUnits);
                db.SaveChanges();

                TempData["Message"] = "<strong>Book has been added successfully. The books are set to have the Branch ID assigned to you. You can manually update Branch ID per stock in Manage Stock.</strong>";
                return RedirectToAction("Index");
            }

            return View(bookTemplate);
        }

        public async Task<JsonResult> SearchISBN(string search)
        {
            WizBooklat.Models.ViewModels.BookData bookDataInitial = new Models.ViewModels.BookData();
            WizBooklat.Models.ViewModels.OpenLibraryBookData bookDataFinal = new Models.ViewModels.OpenLibraryBookData();

            using (HttpClient client = new HttpClient())
            {
                if (ServicePointManager.SecurityProtocol != SecurityProtocolType.Tls12)
                {
                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                }

                try
                {
                    // sample 0451526538
                    string partialUri = "https://openlibrary.org/api/books?bibkeys=ISBN:"+search+"&jscmd=data&format=json";

                    var uri = new Uri(partialUri);
                    var result = await client.GetAsync(uri);
                    string resultContent = await result.Content.ReadAsStringAsync();
                    
                    var data = (JObject)JsonConvert.DeserializeObject(resultContent);

                    if (data.HasValues)
                    {
                        string ISBN = data.First.Path;

                        bookDataInitial = JsonConvert.DeserializeObject<WizBooklat.Models.ViewModels.BookData>(data.First.FirstOrDefault().ToString());
                        bookDataFinal = new WizBooklat.Models.ViewModels.OpenLibraryBookData
                        {
                            ISBN = ISBN,
                            BookData = bookDataInitial
                        };
                    }
                }
                catch (Exception ex)
                {
                    
                }
            }

            if (bookDataFinal.BookData != null)
            {
                return Json(
                    new { result = 1, data = bookDataFinal },
                    JsonRequestBehavior.AllowGet
                );
            }
            else
            {
                return Json(
                    new { result = 0 },
                    JsonRequestBehavior.AllowGet
                );
            }
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
        public ActionResult Edit([Bind(Include = "BookTemplateId,Title,Description,ImageLocation,LoanPeriod,ISBN,OLKey,PublishYear,InitialQuantity,Genres,Authors")] BookTemplate bookTemplate)
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

        public ActionResult Feature(int? id)
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

            bookTemplate.IsFeatured = true;
            db.SaveChanges();
            TempData["Message"] = "Successfully updated info. <strong>" + bookTemplate.Title + "</strong> is now Featured.";
            return RedirectToAction("Index");
        }

        public ActionResult RemoveFeatured(int? id)
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

            bookTemplate.IsFeatured = false;
            db.SaveChanges();
            TempData["Message"] = "Successfully updated info. <strong>" + bookTemplate.Title + "</strong> Featured attribute has been removed.";
            return RedirectToAction("Index");
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
