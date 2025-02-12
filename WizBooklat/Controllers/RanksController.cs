﻿using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using WizBooklat.Models;
using WizBooklat.Models.ViewModels;

namespace WizBooklat.Controllers
{
    public class RanksController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        
        public ActionResult ClaimReward(int? id)
        {
            if (id == null)
            {
                TempData["Error"] = "1";
                TempData["Message"] = "<strong>Failed to claim reward; Reward not found.</strong>";
                return RedirectToAction("ViewRewards");
            }
            Reward reward = db.Rewards.Find(id);
            if (reward == null)
            {
                TempData["Error"] = "1";
                TempData["Message"] = "<strong>Failed to claim reward; Reward not found.</strong>";
                return RedirectToAction("ViewRewards");
            }

            if (User.Identity.IsAuthenticated)
            {
                string userId = User.Identity.GetUserId();

                Claim newClaim = new Claim
                {
                    Code = String.Join("",Guid.NewGuid().ToString().Take(10)),
                    DateCreated = DateTime.UtcNow.AddHours(8),
                    RewardId = reward.RewardId,
                    UserId = userId
                };

                db.Claims.Add(newClaim);
                db.SaveChanges();

                TempData["Message"] = "<strong>Reward claimed successfully; Please use the code; "+ newClaim.Code 
                    +" when claiming your reward.</strong>";
            }
            else
            {
                TempData["Error"] = "1";
                TempData["Message"] = "<strong>Failed to claim reward; please login first.</strong>";
            }

            return RedirectToAction("ViewRewards");
        }

        public ActionResult ViewRewards()
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

            ApplicationUser user = db.Users.Include(u => u.PointHistory).Where(u => u.Id == userId).FirstOrDefault();

            ViewRewardsModel vrm = new ViewRewardsModel
            {
                User = user,
                Ranks = db.Ranks.ToList()
            };

            return View(vrm);
        }

        [HttpPost]
        public ActionResult EditReward(Reward reward)
        {
            if (ModelState.IsValid)
            {
                db.Entry(reward).State = EntityState.Modified;
                db.SaveChanges();
                TempData["Message"] = "<strong>Successfully added Reward.</strong>";
                return RedirectToAction("Index");
            }

            ViewBag.Ranks = db.Ranks.ToList();
            return View(reward);
        }

        public ActionResult EditReward(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Reward reward = db.Rewards.Find(id);
            if (reward == null)
            {
                return HttpNotFound();
            }
            return View(reward);
        }

        [HttpPost]
        public ActionResult AddReward(Reward reward)
        {
            reward.DateCreated = DateTime.UtcNow.AddHours(8);
            if (ModelState.IsValid)
            {
                db.Rewards.Add(reward);
                db.SaveChanges();
                TempData["Message"] = "<strong>Successfully added Reward.</strong>";
                return RedirectToAction("Index");
            }
            
            ViewBag.Ranks = db.Ranks.ToList();
            return View(reward);
        }

        public ActionResult AddReward(int? id)
        {
            Rank rank = db.Ranks.Find(id);
            if (rank != null)
            {
                ViewBag.Rank = rank;
            }
            ViewBag.Ranks = db.Ranks.ToList();

            return View();
        }

        public ActionResult Rewards(int? id)
        {
            if (TempData["Error"] != null)
            {
                ViewBag.Error = TempData["Error"];
            }
            if (TempData["Message"] != null)
            {
                ViewBag.Message = TempData["Message"];
            }

            if (id == null)
            {
                return View(db.Rewards.ToList());
            }
            else
            {
                return View(db.Rewards.Where(r => r.RankId == id.Value).ToList());
            }
        }

        // GET: Ranks
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

            return View(db.Ranks.ToList());
        }

        // GET: Ranks/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Rank rank = db.Ranks.Find(id);
            if (rank == null)
            {
                return HttpNotFound();
            }
            return View(rank);
        }

        // GET: Ranks/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Ranks/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "RankId,Name,Points,Color")] Rank rank)
        {
            if (ModelState.IsValid)
            {
                if (db.Ranks.Any(r=>r.Points == rank.Points))
                {
                    ModelState.AddModelError("Points", "Another Rank entity already exists with the same value.");
                    return View(rank);
                }

                db.Ranks.Add(rank);
                db.SaveChanges();
                TempData["Message"] = "<strong>Successfully added Rank.</strong>";
                return RedirectToAction("Index");
            }

            return View(rank);
        }

        // GET: Ranks/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Rank rank = db.Ranks.Find(id);
            if (rank == null)
            {
                return HttpNotFound();
            }
            return View(rank);
        }

        // POST: Ranks/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "RankId,Name,Points,Color")] Rank rank)
        {
            if (ModelState.IsValid)
            {
                db.Entry(rank).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(rank);
        }

        // GET: Ranks/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Rank rank = db.Ranks.Find(id);
            if (rank == null)
            {
                return HttpNotFound();
            }
            return View(rank);
        }

        // POST: Ranks/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Rank rank = db.Ranks.Find(id);
            db.Ranks.Remove(rank);
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
