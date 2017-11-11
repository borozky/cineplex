using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Cineplex.Data;
using Cineplex.Models;
using Cineplex.Helpers;
using Cineplex.Models.CheckoutViewModel;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Authorization;

namespace Cineplex.Controllers
{
    public class CheckoutController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public CheckoutController(ApplicationDbContext db, 
            UserManager<ApplicationUser> userManager, 
            SignInManager<ApplicationUser> signInManager)
        {
            _db = db;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        [Authorize]
        public IActionResult Index()
        {
            // GUEST ARE NOT ALLOWED TO CHECKOUT
            if (_signInManager.IsSignedIn(User) == false)
            {
                TempData["danger"] = "Only logged in users are allowed to checkout";
                return RedirectToAction("Index", "Home");
            }

            return _Index();
        }

        // This method will be used by GET /Index/ and POST /ProcessPayment/
        private IActionResult _Index(Payment payment = null)
        {
            var cart = HttpContext.Session.Get<Cart>("Cart");
            CheckoutIndexViewModel viewModel = new CheckoutIndexViewModel();

            // ensure cart is created
            if (cart == null)
            {
                cart = new Cart();
                HttpContext.Session.Set<Cart>("Cart", cart);
            }

            viewModel.Cart = cart;

            // cannot proceed to check out if there are remaining seats left unfilled
            if (cart.HasRemainingSeatsLeft())
            {
                TempData["danger"] = "Before you enter the checkout, please complete your seating arrangement first";
                return RedirectToAction("Index", "Cart");
            }

            if (cart.Bookings != null && cart.Bookings.Any())
            {
                var movieIds = cart.Bookings.Select(b => b.Show.MovieId).Distinct().ToArray();
                var movies = _db.Movies.Include(m => m.Images).Where(m => movieIds.Contains(m.Id)).ToList();
                if (movies != null && movies.Any())
                {
                    viewModel.Movies = movies;
                }

                var cinemaIds = cart.Bookings.Select(b => b.Show.CinemaId).Distinct().ToArray();
                var cinemas = _db.Cinemas.Where(c => cinemaIds.Contains(c.Id)).ToList();
                if (cinemas != null && cinemas.Any())
                {
                    viewModel.Cinemas = cinemas;
                }

                var showIds = cart.Bookings.Select(b => b.ShowId).Distinct().ToArray();
                var shows = _db.Shows.Where(s => showIds.Contains(s.Id)).ToList();
                if (shows != null && shows.Any())
                {
                    viewModel.Shows = shows;
                }
            }

            if (payment != null)
            {
                viewModel.Payment = payment;
            }

            return View("Index",viewModel);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> ProcessPayment([Bind("CreditCardNumber, ExpiryYear, ExpiryMonth, CVC, CardHolder")]Payment payment, string returnUrl = null)
        {
            // process payment
            if (ModelState.IsValid)
            {
                var cart = HttpContext.Session.Get<Cart>("Cart");


                var bookings = cart.Bookings;
                var pricingTypes = await _db.PricingTypes.ToListAsync();

                // instead of saving the existing booking, create a new booking
                var bookingsToBeSaved = new List<Booking>();
                foreach (Booking booking in bookings)
                {
                    Booking b = new Booking();

                    // create multiple tickets based on qty
                    var tickets = new List<Ticket>();

                    foreach (Ticket ticket in booking.Tickets)
                    {
                        for (int i = 0; i < ticket.Quantity; i++)
                        {
                            tickets.Add(new Ticket
                            {
                                PricingType = pricingTypes.First(p => p.Id == ticket.PricingTypeId),
                                PricingTypeId = ticket.PricingTypeId
                            });
                        }
                    }

                    b.Tickets = tickets;
                    b.ShowId = booking.ShowId;
                    b.Seats = booking.Seats;

                    bookingsToBeSaved.Add(b);
                }

                // generate order
                var order = new Order
                {
                    Bookings = bookingsToBeSaved,
                    OrderedById = _userManager.GetUserId(User),
                    Total = (decimal) cart.CalculateTotal()
                };
                
                _db.Orders.Add(order);
                _db.Entry(order).Property("CreatedAt").CurrentValue = DateTime.UtcNow;
                await _db.SaveChangesAsync();

                // clear cart
                cart = new Cart();
                HttpContext.Session.Set("Cart", cart);

                return RedirectToAction("Details", "Orders", new { Id = order.Id });
            }

            // invalid payment
            TempData["danger"] = "Cannot proceed with the payment, validation errors has occured";
            return _Index();
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