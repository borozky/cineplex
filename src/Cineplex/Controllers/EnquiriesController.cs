using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Cineplex.Data;
using Cineplex.Models;
using Microsoft.AspNetCore.Hosting;
using Cineplex.Attributes;
using Microsoft.AspNetCore.Authorization;

namespace Cineplex.Controllers
{
    [Authorize(Roles = "admin")]
    public class EnquiriesController : Controller
    {
        private readonly ApplicationDbContext _db;
        private IHostingEnvironment _env;

        public EnquiriesController(ApplicationDbContext context, IHostingEnvironment env)
        {
            _db = context;
            _env = env;
        }

        // GET: Enquiries
        public async Task<IActionResult> Index()
        {
            var events = _db.Enquiries
                .Include(e => e.Event);
            return View(await events.ToListAsync());
        }

        // GET: Enquiries/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var enquiry = await _db.Enquiries.SingleOrDefaultAsync(m => m.Id == id);
            if (enquiry == null)
            {
                return NotFound();
            }

            return View(enquiry);
        }

        // GET: Enquiries/Create
        [AllowAnonymous]
        public IActionResult Create()
        {
            ViewData["EventId"] = new SelectList(_db.Events, "Id", "Details");
            return View();
        }

        // POST: Enquiries/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ExportModelState]
        [AllowAnonymous]
        public async Task<IActionResult> Create([Bind("Id,EventId,From,Message,To")] Enquiry enquiry)
        {
            if (ModelState.IsValid)
            {
                _db.Add(enquiry);
                await _db.SaveChangesAsync();
                TempData["success"] = "You enquiry has been saved";
                return RedirectToAction("Details", "Events", new { Id = enquiry.EventId });
            }
            ViewData["EventId"] = new SelectList(_db.Events, "Id", "Details", enquiry.EventId);
            return RedirectToAction("Details", "Events", new { Id = enquiry.EventId });
        }

        private bool EnquiryExists(int id)
        {
            return _db.Enquiries.Any(e => e.Id == id);
        }
    }
}
