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
using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Authorization;
using Cineplex.Attributes;

namespace Cineplex.Controllers
{
    [Authorize(Roles = "admin")]
    public class EventsController : Controller
    {
        private readonly ApplicationDbContext _db;
        private IHostingEnvironment _env;

        // ASSUMPTIONS
        // --------------------------------------
        // 1. Events CANNOT be deleted
        // 2. Events can be marked as CANCELLED(0), ACTIVE(1), PENDING(2)
        //
        // RESTRICTIONS
        // -------------------------------
        // 1. Requires LOGIN to Create and Edit
        //
        // CAN HAVE....
        // ----------------------------
        // 1. Many images under wwwroot/images/events

        public EventsController(ApplicationDbContext context, IHostingEnvironment env)
        {
            _db = context;
            _env = env; 
        }

        // GET: Events
        [AllowAnonymous]
        public async Task<IActionResult> Index()
        {
            var events = await _db.Events
                .Include(e => e.Images)
                .AsNoTracking()
                .ToListAsync();

            return View(events);
        }

        // GET: Events/Details/5
        [AllowAnonymous]
        [ImportModelState]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var @event = await _db.Events
                .Include(e => e.Images)
                .SingleOrDefaultAsync(m => m.Id == id);

            if (@event == null)
            {
                return NotFound();
            }

            return View(@event);
        }

        // GET: Events/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Events/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Details,StartDate,Title")] Event @event, List<IFormFile> files)
        {
            string folder = "images/EventImages/";
            var normalImagesPath = Path.Combine(_env.WebRootPath, folder);  // _env is injected via DI 
            List<Image> images = new List<Image>();

            // upload images
            try
            {
                foreach (var file in files)
                {
                    // TODO: Robust file validation

                    // exit if not an image
                    var extension = Path.GetExtension(file.FileName);
                    var allowed = new string[] { ".jpg", ".png", ".jpeg", ".gif"  };

                    if (! allowed.Contains(extension))
                    {
                        TempData["danger"] = $"File \"{file.FileName}\" has an invalid extension";
                        return View(@event);
                    }

                    if (file.Length > 0)
                    {
                        using (var stream = new FileStream(Path.Combine(normalImagesPath, file.FileName), FileMode.Create))
                        {
                            await file.CopyToAsync(stream);
                            var location = folder + file.FileName;
                            images.Add(new Image { FileName = location });
                        }
                    }
                }
            }
            catch (Exception e)
            {
                TempData["danger"] = e.Message;
                return View(@event);
            }


            if (ModelState.IsValid)
            {
                @event.Images = images;

                _db.Add(@event);
                await _db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(@event);
        }

        // GET: Events/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var @event = await _db.Events.SingleOrDefaultAsync(m => m.Id == id);
            if (@event == null)
            {
                return NotFound();
            }
            return View(@event);
        }

        // POST: Events/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Details,StartDate,Title")] Event @event)
        {
            if (id != @event.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _db.Update(@event);
                    await _db.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EventExists(@event.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("Index");
            }
            return View(@event);
        }

        // GET: Events/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var @event = await _db.Events.SingleOrDefaultAsync(m => m.Id == id);
            if (@event == null)
            {
                return NotFound();
            }

            return View(@event);
        }

        // POST: Events/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var @event = await _db.Events.SingleOrDefaultAsync(m => m.Id == id);
            _db.Events.Remove(@event);
            await _db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        private bool EventExists(int id)
        {
            return _db.Events.Any(e => e.Id == id);
        }
    }
}
