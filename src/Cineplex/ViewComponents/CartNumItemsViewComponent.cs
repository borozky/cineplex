using Cineplex.Data;
using Cineplex.Helpers;
using Cineplex.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cineplex.ViewComponents
{
    [ViewComponent]
    public class CartNumItemsViewComponent : ViewComponent
    {
        private ApplicationDbContext _db;
        public CartNumItemsViewComponent(ApplicationDbContext context)
        {
            _db = context;
        }

        public IViewComponentResult Invoke()
        {
            var cart = HttpContext.Session.Get<Cart>("Cart");

            // ensure cart is created
            if (cart == null)
            {
                cart = new Cart
                {
                    Bookings = new List<Booking>(),
                    Shows = new List<Show>()
                };
                HttpContext.Session.Set<Cart>("Cart", cart);
            }
            return View(cart);
        }
    }
}
