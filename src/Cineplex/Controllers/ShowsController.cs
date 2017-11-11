using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Cineplex.Data;
using Cineplex.Models;
using Microsoft.AspNetCore.Http;
using Cineplex.Helpers;
using Cineplex.Attributes;
using Microsoft.AspNetCore.Authorization;

namespace Cineplex.Controllers
{
    [Authorize(Roles = "admin")]
    public class ShowsController : Controller
    {
        private readonly ApplicationDbContext _db;

        public ShowsController(ApplicationDbContext context)
        {
            _db = context;    
        }

        // GET: Shows
        [AllowAnonymous]
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _db.Shows.Include(s => s.Cinema).Include(s => s.Movie);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Shows/Details/5
        [AllowAnonymous]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var show = await _db.Shows.SingleOrDefaultAsync(m => m.Id == id);
            if (show == null)
            {
                return NotFound();
            }

            return View(show);
        }

        // GET: Shows/Create
        public IActionResult Create()
        {
            ViewData["CinemaId"] = new SelectList(_db.Cinemas, "Id", "Location");
            ViewData["MovieId"] = new SelectList(_db.Movies, "Id", "Title");
            return View();
        }

        // POST: Shows/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ExportModelState]
        public async Task<IActionResult> Create([Bind("Id,CinemaId,MovieId,SessionTime")] Show show, string returnUrl = null)
        {
            if (ModelState.IsValid)
            {
                _db.Add(show);
                await _db.SaveChangesAsync();

                if (returnUrl != null)
                {
                    TempData["success"] = "Show has been created";
                    return RedirectToLocal(returnUrl);
                }
                return RedirectToAction("Index");
            }

            ViewData["CinemaId"] = new SelectList(_db.Cinemas, "Id", "Location", show.CinemaId);
            ViewData["MovieId"] = new SelectList(_db.Movies, "Id", "Title", show.MovieId);

            return returnUrl != null ? RedirectToLocal(returnUrl) : RedirectToAction("Index");
        }

        // GET: Shows/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var show = await _db.Shows.SingleOrDefaultAsync(m => m.Id == id);
            if (show == null)
            {
                return NotFound();
            }
            ViewData["CinemaId"] = new SelectList(_db.Cinemas, "Id", "Location", show.CinemaId);
            ViewData["MovieId"] = new SelectList(_db.Movies, "Id", "Title", show.MovieId);
            return View(show);
        }

        // POST: Shows/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ExportModelState]
        public async Task<IActionResult> Edit(int id, [Bind("Id,CinemaId,MovieId,SessionTime")] Show show, string returnUrl = null)
        {
            if (id != show.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _db.Update(show);
                    await _db.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ShowExists(show.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }

                if (returnUrl != null)
                {
                    TempData["success"] = "Show has been updated";
                    return RedirectToLocal(returnUrl);
                }
                return RedirectToAction("Index");
            }

            ViewData["CinemaId"] = new SelectList(_db.Cinemas, "Id", "Location", show.CinemaId);
            ViewData["MovieId"] = new SelectList(_db.Movies, "Id", "Title", show.MovieId);
            return RedirectToLocal(returnUrl);
        }

        // GET: Shows/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var show = await _db.Shows.SingleOrDefaultAsync(m => m.Id == id);
            if (show == null)
            {
                return NotFound();
            }

            return View(show);
        }

        // POST: Shows/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var show = await _db.Shows.SingleOrDefaultAsync(m => m.Id == id);
            _db.Shows.Remove(show);
            await _db.SaveChangesAsync();
            return RedirectToAction("Index");
        }
        
        private bool ShowExists(int id)
        {
            return _db.Shows.Any(e => e.Id == id);
        }

        private IActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction("Index"); ;
            }
        }


    }
}
