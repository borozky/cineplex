using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Cineplex.Models;
using Cineplex.Data;
using Microsoft.EntityFrameworkCore;
using Cineplex.Helpers;
using Cineplex.Models.CartViewModels;
using Newtonsoft.Json;

namespace Cineplex.Controllers
{
    public class CartController : Controller
    {
        private readonly ApplicationDbContext _db;
        
        public CartController(ApplicationDbContext db)
        {
            _db = db;
        }
        
        // The cart page
        public async Task<IActionResult> Index()
        {
            var cart = HttpContext.Session.Get<Cart>("Cart");
            List<PricingType> pricingTypes = await _db.PricingTypes.ToListAsync();

            var viewModel = new CartIndexViewModel
            {
                Cart = cart,
                PricingTypes = pricingTypes
            };
            
            if (cart != null && cart.Bookings != null)
            {
                var movieIds = cart.Bookings.Select(i => i.Show.MovieId).Distinct().ToArray();

                var movies = await _db.Movies
                    .Include(m => m.Shows)
                    .Include(m => m.Images)
                    .Where(m => movieIds.Contains(m.Id))
                    .ToListAsync();
                

                foreach (Movie m in movies)
                {
                    foreach (Show s in m.Shows)
                    {
                        var found = cart.Bookings.FirstOrDefault(b => b.ShowId == s.Id);
                        if (found != null)
                        {
                            if (s.Bookings == null)
                            {
                                s.Bookings = new List<Booking>();
                            }
                            s.Bookings.Add(found);
                        }
                    }
                }

                viewModel.Movies = movies;
            }
            
            return View(viewModel);
        }
        
        

        //[HttpPost]
        //public async Task<IActionResult> UpdateCart([Bind("Id, MovieId, CinemaId, SessionTime, TicketQuantities")] Show show)
        //{

        //    if (ModelState.IsValid)
        //    {

        //        var dbShow = await _db.Shows.SingleOrDefaultAsync(s => s.Id == show.Id);

        //        if (show == null)
        //        {
        //            return NotFound();
        //        }
            
        //        var cart = HttpContext.Session.Get<Cart>("Cart");
        //        if (cart == null)
        //        {
        //            cart = new Cart();
        //            cart.Shows = new List<Show>();
        //        }
                
        //        var found = cart.Shows.FirstOrDefault(s => s.Id == show.Id);
        //        if (found != null)
        //        {
        //            found.TicketQuantities = show.TicketQuantities;
        //            TempData["success"] = "Cart successfully updated";
        //            HttpContext.Session.Set<Cart>("Cart", cart);
        //            return RedirectToAction("Index");
        //            //return returnUrl != null ? RedirectToLocal(returnUrl) : RedirectToAction("Index");

        //        }
        //        else
        //        {
        //            cart.Shows.Add(show);
        //            HttpContext.Session.Set<Cart>("Cart", cart);
        //            TempData["success"] = "Booking has successfully added to your cart";
        //            return RedirectToAction("Details", "Movie", new { Id = show.MovieId });
                    
        //        }
        //    }

        //    return NotFound();
            
        //}
        
        [HttpPost]
        public async Task<IActionResult> AddBookingToCart([Bind("ShowId, Tickets, Quantity, Seats")] Booking booking, string returnUrl = null)
        {

            if (ModelState.IsValid)
            {
                var cart = HttpContext.Session.Get<Cart>("Cart");
                
                if (cart == null)
                {
                    cart = new Cart();
                    cart.Bookings = new List<Booking>();
                }

                // check for the qty of tickets
                var tickets = cart.Bookings.Where(b => b.ShowId != booking.ShowId).Select(b => b.TotalNumTickets()).Sum();
                if (tickets + booking.TotalNumTickets() > 5)
                {
                    TempData["danger"] = "Only 5 tickets are allowed";
                    return returnUrl != null ? RedirectToLocal(returnUrl) : RedirectToAction("Index");
                }

                var show = await _db.Shows
                    .Include(s => s.Movie)
                    .Include(s => s.Cinema)
                    .FirstOrDefaultAsync(s => s.Id == booking.ShowId);

                // check if this show is about to start
                if (show.IsAboutToStart())
                {
                    TempData["danger"] = "Cannot book to this show anymore because it is about to start";
                    return returnUrl != null ? RedirectToLocal(returnUrl) : RedirectToAction("Index");
                }

                booking.Show = show;

                var ticketTypes = await _db.PricingTypes.ToListAsync();


                var found = cart.Bookings.FirstOrDefault(b => b.ShowId == booking.ShowId);
                if (found != null)
                {
                    // FILTER BOOKINGS THAT HAS NO QTY
                    // update booking
                    found.Tickets = booking.Tickets;

                    // ensure pricing types information are populated
                    found.Tickets.ForEach(t => t.PricingType = ticketTypes.FirstOrDefault(p => p.Id == t.PricingTypeId));

                    foreach (var ticket in found.Tickets)
                    {
                        if (ticket.PricingType == null)
                        {
                            ticket.PricingType = ticketTypes.FirstOrDefault(t => t.Id == ticket.PricingTypeId);
                        }
                    }

                    HttpContext.Session.Set<Cart>("Cart", cart);
                    TempData["success"] = "You booking was successfully updated";
                    return returnUrl != null ? RedirectToLocal(returnUrl) : RedirectToAction("Index");
                }
                else
                {
                    // ensure pricing types information are populated
                    booking.Tickets.ForEach(t => t.PricingType = ticketTypes.FirstOrDefault(p => p.Id == t.PricingTypeId));

                    // add booking
                    cart.Bookings.Add(booking);
                    HttpContext.Session.Set<Cart>("Cart", cart);
                    TempData["success"] = "You booking was successfully added to the cart";
                    return returnUrl != null ? RedirectToLocal(returnUrl) : RedirectToAction("Details", "Movies", new { Id = show.MovieId });
                }

            }
            else
            {
                var errors = ModelState.Values.SelectMany(x => x.Errors).Select(x => x.ErrorMessage).ToArray();
                TempData["danger"] = string.Join(", ", errors);
                return returnUrl != null ? RedirectToLocal(returnUrl) : RedirectToAction("Index");
            }

        }
        
        [HttpPost]
        public IActionResult UpdateSeating(int? ShowId, [Bind("SeatNumber")]List<Seat> seats, string returnUrl = null)
        {
            // fail early
            if (ShowId == null)
            {
                return NotFound();
            }

            var cart = HttpContext.Session.Get<Cart>("Cart");
            if (cart != null)
            {
                if (cart.Bookings != null)
                {
                    // can only update seating when the booking was found in the cart
                    var found = cart.Bookings.FirstOrDefault(b => b.ShowId == ShowId);
                    if (found != null)
                    {
                        // seat can be null, so we need to filter
                        seats = seats.Where(s => s != null && s.SeatNumber > 0).ToList();

                        int totalSeats = found.Tickets.Select(t => t.Quantity).Sum();
                        
                        // CHECK: seat must not be greater than the number of tickets
                        if (seats.Count() <= totalSeats)
                        {

                            // update seating on that booking
                            found.Seats = seats;
                            HttpContext.Session.Set<Cart>("Cart", cart);

                            var remainingSeats = totalSeats - seats.Count();
                            if (remainingSeats > 0)
                            {
                                TempData["suscess"] = $"Your seating arrangement was successfully updated ({remainingSeats} seats left)";
                            }
                            else
                            {
                                TempData["success"] = "Your seating arrangement was successfully updated";
                            }
                            // redirect to cart page
                            return RedirectToAction("Index");
                        }
                        else
                        {
                            TempData["danger"] = $"You picked too many seats";
                            return returnUrl != null ? RedirectToLocal(returnUrl) : RedirectToAction("Index");
                        }

                        

                    }
                }
            }
            return NotFound();

        }

        [HttpPost]
        public IActionResult RemoveBooking(int? ShowId, string returnUrl = null)
        {
            if (ShowId == null)
            {
                return NotFound();
            }

            var cart = HttpContext.Session.Get<Cart>("Cart");
            var found = cart.Bookings.FirstOrDefault(b => b.ShowId == (int)ShowId);
            if (found != null)
            {
                cart.Bookings.Remove(found);
                HttpContext.Session.Set<Cart>("Cart", cart);
                TempData["success"] = "Booking has been removed from your cart";
                return returnUrl != null ? RedirectToLocal(returnUrl) : RedirectToAction("Index");
            }

            return NotFound();

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