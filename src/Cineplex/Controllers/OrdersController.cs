using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Cineplex.Data;
using Cineplex.Models;
using Cineplex.Models.OrderViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace Cineplex.Controllers
{

    [Authorize]
    public class OrdersController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        // An order can have at least 5 tickets

        public OrdersController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
        {
            _db = context;
            _signInManager = signInManager;
            _userManager = userManager;
             
        }

        // GET: Orders
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _db.Orders.Include(o => o.OrderedBy);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Orders/Details/5
        
        public async Task<IActionResult> Details(int? id)
        {

            if (id == null)
            {
                return NotFound();
            }

            var order = await _db.Orders.SingleOrDefaultAsync(m => m.Id == id);
            if (order == null)
            {
                return NotFound();
            }

            // Except for admins, if you don't own this order, don't look at it
            var userId = _userManager.GetUserId(User);
            if (User.IsInRole("admin") == false)
            {
                if (order.OrderedById != userId)
                {
                    return NotFound();
                }
            }
            

            if (order == null)
            {
                return NotFound();
            }

            IQueryable<Booking> bookings = _db.Bookings
                        .Include(b => b.Show)
                            .ThenInclude(s => s.Movie)
                        .Include(b => b.Show)
                            .ThenInclude(s => s.Cinema)
                        .Include(b => b.Tickets)
                            .ThenInclude(t => t.PricingType)
                        .Include(b => b.Seats)
                        .Where(b => b.OrderId == order.Id)
                        .AsNoTracking();

            OrderDetailsViewModel viewModel = new OrderDetailsViewModel
            {
                Order = order,
                DateCreated = (DateTime)_db.Entry(order).Property("CreatedAt").CurrentValue
            };

            viewModel.Order.Bookings = await bookings.ToListAsync();

            return View(viewModel);
        }
    }
}
