using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Cineplex.Data;
using Cineplex.Models;
using Cineplex.Helpers;
using Cineplex.Models.BookingViewModels;

namespace Cineplex.Controllers
{
    public class BookingsController : Controller
    {
        private readonly ApplicationDbContext _db;

        public BookingsController(ApplicationDbContext context)
        {
            _db = context;    
        }
        // BOOKINGSCONTROLLER REQUIRES LOGIN!!!!!!!
        // BOOKINGS cannot be created by user. It is created by the system after successfull order
        // BOOKINGS ARE UNMODIFIABLE ONCE CREATED
        // BOOKINGS normally displayed from ORDERS controller, but for now let's DISPLAY ALL BOOKINGS by this user
        //---------------
        // ASSUMPTIONS
        // Bookings cannot be cancelled
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _db.Bookings.Include(b => b.Order).Include(b => b.Show);
            return View(await applicationDbContext.ToListAsync());
        }

        // We can only view the booking information
        // ---- Show (incl. Cinema & the Movie we are booked into)
        // ---- Date booked
        // ---- Expiry (derived from Show.End)
        // ---- Cost of booking (with PricingType model)
        // GET: Bookings/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var booking = await _db.Bookings.SingleOrDefaultAsync(m => m.Id == id);
            if (booking == null)
            {
                return NotFound();
            }

            return View(booking);
        }

        

        //// GET: Bookings/Create
        //public IActionResult Create()
        //{
        //    ViewData["OrderId"] = new SelectList(_context.Orders, "Id", "Id");
        //    ViewData["ShowId"] = new SelectList(_context.Shows, "Id", "Id");
        //    return View();
        //}

        // POST: Bookings/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,OrderId,SeatNumber,ShowId")] Booking booking)
        {
            if (ModelState.IsValid)
            {
                _db.Add(booking);
                await _db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewData["OrderId"] = new SelectList(_db.Orders, "Id", "Id", booking.OrderId);
            ViewData["ShowId"] = new SelectList(_db.Shows, "Id", "Id", booking.ShowId);
            return View(booking);
        }

        // GET: Bookings/Edit/5
        public async Task<IActionResult> Edit(int? ShowId)
        {
            var cart = HttpContext.Session.Get<Cart>("Cart");

            if (ShowId != null && cart != null)
            {
                var booking = cart.Bookings.FirstOrDefault(b => b.ShowId == ShowId);
                if (booking != null)
                {
                    if (booking.Tickets != null && booking.Tickets.Count() > 0)
                    {
                        // get booking information
                        BookingEditViewModel viewModel = new BookingEditViewModel();

                        viewModel.Booking = booking;
                        int[] seatsUsed = new int[0];

                        if (booking.Seats != null)
                        {
                            seatsUsed = booking.Seats.Select(s => s.SeatNumber).ToArray();
                        }

                        var existingBookingIds = await _db.Bookings.Where(b => b.ShowId == ShowId).Select(b => b.Id).ToArrayAsync();

                        var show = await _db.Shows
                            .Include(s => s.Cinema)
                            .Include(s => s.Movie)
                            .FirstOrDefaultAsync(s => s.Id == ShowId);

                        var reservedSeatNumbers = await _db.Seats
                            .Where(s => existingBookingIds.Contains(s.BookingId))
                            .Select(s => s.SeatNumber).Distinct().ToArrayAsync();

                        viewModel.SeatsUsed = seatsUsed;
                        viewModel.Reserved = reservedSeatNumbers;
                        viewModel.Show = show;

                        return View(viewModel);

                    }
                }
            }

            return NotFound();
        }
        

        // POST: Bookings/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,OrderId,SeatNumber,ShowId")] Booking booking)
        {
            if (id != booking.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _db.Update(booking);
                    await _db.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BookingExists(booking.Id))
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
            ViewData["OrderId"] = new SelectList(_db.Orders, "Id", "Id", booking.OrderId);
            ViewData["ShowId"] = new SelectList(_db.Shows, "Id", "Id", booking.ShowId);
            return View(booking);
        }

        // GET: Bookings/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var booking = await _db.Bookings.SingleOrDefaultAsync(m => m.Id == id);
            if (booking == null)
            {
                return NotFound();
            }

            return View(booking);
        }

        // POST: Bookings/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var booking = await _db.Bookings.SingleOrDefaultAsync(m => m.Id == id);
            _db.Bookings.Remove(booking);
            await _db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        private bool BookingExists(int id)
        {
            return _db.Bookings.Any(e => e.Id == id);
        }
    }
}
