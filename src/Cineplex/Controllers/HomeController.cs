using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Cineplex.Data;
using Microsoft.EntityFrameworkCore;
using Cineplex.Models.HomeViewModels;
using Microsoft.AspNetCore.Http;
using Cineplex.Models;
using Cineplex.Helpers;

namespace Cineplex.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _db;

        public HomeController(ApplicationDbContext db)
        {
            _db = db;
        }
        
        public async Task<IActionResult> Index()
        {
            var movies = await _db.Movies
                .Include(m => m.Images)
                .Include(m => m.Shows)
                .AsNoTracking()
                .ToListAsync();

            var cinemas = await _db.Cinemas
                .Include(m => m.Images)
                .AsNoTracking()
                .ToListAsync();

            var events = await _db.Events
                .ToListAsync();

            var shows = await _db.Shows
                .Include(s => s.Cinema)
                .AsNoTracking()
                .ToListAsync();

            var viewModel = new HomeIndexViewModel
            {
                Movies = movies,
                Cinemas = cinemas,
                Events = events,
                Shows = shows
            };
           
            ViewData["title"] = "Home";
            
            return View(viewModel);
        }

        [Route("about")]
        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";
            ViewData["Title"] = "About";
            return View();
        }

        [Route("contact")]
        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";
            ViewData["Title"] = "Contact";
            return View();
        }


        public IActionResult Error()
        {
            return View();
        }
        
    }
}
