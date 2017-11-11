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
using Cineplex.Models.CinemaViewModels;
using Cineplex.Repositories;
using Cineplex.Attributes;
using Microsoft.AspNetCore.Authorization;

namespace Cineplex.Controllers
{
    [Authorize(Roles = "admin")]
    public class CinemasController : Controller
    {
        private readonly ApplicationDbContext _db;
        private IHostingEnvironment _env;
        private ICinemasRepository cinemarepo;

        public CinemasController(ApplicationDbContext context, IHostingEnvironment env, ICinemasRepository cinemaRepository)
        {
            _db = context;
            _env = env;
            cinemarepo = cinemaRepository;    
        }

        // CONDITIONS:
        // Retrive all cinemas information, each with 
        // --- all images, 
        // --- previous 10 shows
        // --- currently running show
        // --- upcoming 10 shows

        // GET: Cinemas
        [AllowAnonymous]
        public async Task<IActionResult> Index()
        {
            var cinemas = await _db.Cinemas
                    .Include(c => c.Images)
                    .AsNoTracking()
                    .ToListAsync();

            var viewModel = new CinemaIndexViewModel { Cinemas = cinemas };

            return View( viewModel );
        }
        
        [AllowAnonymous]
        [HttpGet("Cinemas/{id}/Movies/")]
        public IActionResult Movies(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cinema = cinemarepo.Find((int)id);
            if (cinema == null)
            {
                return NotFound();
            }

            var movies = cinemarepo.GetMovies((int)id).ToList();
            if (movies == null)
            {
                return NotFound();
            }

            CinemaMoviesViewModel viewModel = new CinemaMoviesViewModel
            {
                Movies = movies,
                Cinema = cinema
            };

            return View(viewModel);
            

        }


        // GET: Cinemas/Details/5
        [AllowAnonymous]
        [ImportModelState]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cinema = await _db.Cinemas.SingleOrDefaultAsync(m => m.Id == id);
            if (cinema == null)
            {
                return NotFound();
            }

            return View(cinema);
        }

        // CREATING ADDITIONAL CINEMAS IS NOT REQUIRED IN CURRENT SPECS

        //// GET: Cinemas/Create
        //public IActionResult Create()
        //{
        //    return View();
        //}

        //// POST: Cinemas/Create
        //// To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        //// more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Create([Bind("Id,Location,Seats")] Cinema cinema)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        _context.Add(cinema);
        //        await _context.SaveChangesAsync();
        //        return RedirectToAction("Index");
        //    }
        //    return View(cinema);
        //}


        // RESTRICTIONS:
        // CANNOT edit Cinema location
        // CANNOT edit Cinema's number of seats NOT SURE
        // CAN add images
        // CAN remove existing ones NOT SURE
        // GET: Cinemas/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cinema = await _db.Cinemas.SingleOrDefaultAsync(m => m.Id == id);
            if (cinema == null)
            {
                return NotFound();
            }
            return View(cinema);
        }


        // POST: Cinemas/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ExportModelState]
        public async Task<IActionResult> Edit(int id, [Bind("Id, Location, Details, Seats")] Cinema cinema, List<IFormFile> files)
        {

            if (id != cinema.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {

                List<Image> images = new List<Image>();
                try
                {
                    images = await UploadImages(files);
                }
                catch (Exception e)
                {
                    TempData["danger"] = $"Error uploading images: {e.Message}";
                    return RedirectToAction("Index");
                }
                cinema.Images = images;

                try
                {
                    _db.Update(cinema);
                    await _db.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CinemaExists(cinema.Id))
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
            return RedirectToAction("Index");
        }

        private async Task<List<Image>> UploadImages(List<IFormFile> files)
        {
            string folder = "images/CinemaImages/";
            var normalImagesPath = Path.Combine(_env.WebRootPath, folder);
            List<Image> images = new List<Image>();

            foreach (var file in files)
            {
                // allow JPG, PNG, JPEG, GIF ONLY
                var extension = Path.GetExtension(file.FileName);
                var allowed = new string[] { ".jpg", ".png", ".jpeg", ".gif" };

                if (!allowed.Contains(extension))
                {
                    throw new Exception("Invalid file extension");
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
                else
                {
                    throw new IOException("File has no content");
                }
            }

            return images;
        }

        // DELETING CINEMAS IS NOT POSSIBLE

        private bool CinemaExists(int id)
        {
            return _db.Cinemas.Any(e => e.Id == id);
        }
    }
}
