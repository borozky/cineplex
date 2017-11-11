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
using Microsoft.AspNetCore.Http;
using System.IO;
using Microsoft.Net.Http.Headers;
using Microsoft.AspNetCore.Hosting;
using Cineplex.Models.MovieViewModels;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Cineplex.Attributes;
using Microsoft.AspNetCore.Authorization;
using Cineplex.Repositories;

namespace Cineplex.Controllers
{
    [Authorize(Roles = "admin")]
    public class MoviesController : Controller
    {
        private readonly ApplicationDbContext _db;
        private IHostingEnvironment _env;
        private IMoviesRepository repo;

        public MoviesController(ApplicationDbContext context, IHostingEnvironment env, IMoviesRepository repository)
        {
            _db = context;
            _env = env;
            repo = repository;
        }

        // GET: Movies
        [AllowAnonymous]
        public async Task<IActionResult> Index()
        {
            return await _Index();

        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Search(string criteria = null, string keyword = "")
        {
            if (criteria == null)
            {
                return NotFound();
            }

            criteria = criteria.ToLower();

            var movies = new List<Movie>();

            switch (criteria)
            {
                case "cinema":
                    var moviesByCinema = repo.SearchByCinema(keyword);
                    if (moviesByCinema != null)
                    {
                        movies = moviesByCinema.ToList();

                    }
                    break;
                case "title":
                    var moviesByTitle = repo.SearchByTitle(keyword);
                    if (moviesByTitle != null)
                    {
                        movies = moviesByTitle.ToList();
                    }
                    break;
            }

            if (movies == null)
            {
                movies = new List<Movie>();
            }

            var ratings = await _db.Ratings.ToListAsync();
            ViewBag.Ratings = ratings;

            ViewBag.Title = $"Found {movies.Count()} movie(s)";
            return View(movies);



        }

        // Allow model state errors to be passed into index
        private async Task<IActionResult> _Index(Movie movie = null)
        {
            var movies = await _db.Movies
                .Include(m => m.Images)
                .Include(m => m.Shows)
                    .ThenInclude(show => show.Cinema)
                .AsNoTracking()
                .ToListAsync();

            var ratings = await _db.Ratings.ToListAsync();
            ViewBag.Ratings = ratings;

            ViewBag.EditedMovie = movie != null ? movie : new Movie();

            return View("Index", movies);
        }


        // GET: Movies/Details/5
        [ImportModelState]
        [AllowAnonymous]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            // fetch all movies including the shows
            var movie = await _db.Movies
                .Include(m => m.Shows)
                    .ThenInclude(s => s.Cinema)
                .Include(m => m.Images)
                .Include(m => m.Rating)
                .AsNoTracking()
                .SingleOrDefaultAsync(m => m.Id == id);

            // exit immediately if not found
            if (movie == null)
            {
                return NotFound();
            }

            // retrieve the cart, we will allow updating cart within movie page
            var cart = HttpContext.Session.Get<Cart>("Cart");

            // ensure cart is not null
            if (cart == null){ cart = new Cart { Bookings = new List<Booking>(), Shows = new List<Show>() }; }

            var pricingTypes = await _db.PricingTypes.ToListAsync();
            var shows = movie.Shows;

            // merge cart with movies
            if (cart != null)
            {
                var movieIds = cart.Bookings.Select(i => i.Show.MovieId).Distinct().ToArray();
                var movies = await _db.Movies.Include(m => m.Shows).Where(m => movieIds.Contains(m.Id)).ToListAsync();

                foreach (Show s in shows)
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

            var ratings = await _db.Ratings.ToListAsync();

            var cinemas = await _db.Cinemas.ToListAsync();

            var viewModel = new MovieDetailsViewModel
            {
                Movie = movie,
                Cart = cart,
                Shows = shows,
                PricingTypes = pricingTypes,
                Cinemas = cinemas,
                Ratings = ratings
            };
            return View(viewModel);
        }
        
        // GET: Movies/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Movies/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Description,Rating,Title")] Movie movie, List<IFormFile> files)
        {
            
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

                movie.Images = images;

                _db.Add(movie);
                await _db.SaveChangesAsync();
                TempData["success"] = "New movie has been added";
                return RedirectToAction("Index");
            }
            // pass all errors into home page since we decided to edit on home page
            return await _Index(movie);
        }
        

        // GET: Movies/Edit/5
        public async Task<IActionResult> Edit(int? id, string returnUrl = null)
        {
            if (id == null)
            {
                return NotFound();
            }

            var movie = await _db.Movies.SingleOrDefaultAsync(m => m.Id == id);
            if (movie == null)
            {
                return NotFound();
            }
            return View(movie);
        }

        // POST: Movies/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ExportModelState]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Description, Duration ,RatingId,Title")] Movie movie, string returnUrl = null)
        {
            if (id != movie.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if (movie.RatingId <= 0)
                    {
                        movie.RatingId = null;
                    }
                    _db.Update(movie);
                    await _db.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MovieExists(movie.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                TempData["success"] = "Movie successfully updated";
                return returnUrl != null ? RedirectToLocal(returnUrl) : RedirectToAction("Index");
            }

            return returnUrl != null ? RedirectToLocal(returnUrl) : View(movie);
        }

        // GET: Movies/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var movie = await _db.Movies.SingleOrDefaultAsync(m => m.Id == id);
            if (movie == null)
            {
                return NotFound();
            }

            return View(movie);
        }

        // POST: Movies/Delete/5
        [HttpPost, ActionName("Remove")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var movie = await _db.Movies.Include(m => m.Images).SingleOrDefaultAsync(m => m.Id == id);
            _db.Movies.Remove(movie);
            await _db.SaveChangesAsync();
            return RedirectToAction("Index");
        }


        [HttpPost]
        public async Task<IActionResult> RemoveImage(int? MovieId, int? ImageId)
        {
            if (MovieId == null || ImageId == null)
            {
                return BadRequest(new { status = "fail", message = "Movie and Image IDs are required" });
            }

            var movie = await _db.Movies.Include(m => m.Images).FirstOrDefaultAsync(m => m.Id == MovieId);
            if (movie != null)
            {
                if (movie.Images != null)
                {
                    var image = movie.Images.FirstOrDefault(i => i.Id == ImageId);
                    if (image != null)
                    {
                        string folder = "images/MovieImages/";
                        var normalImagesPath = Path.Combine(_env.WebRootPath, folder);

                        return Ok(new { status = "success", message = $"Folder : {folder}, NormalImagePath: {normalImagesPath}" });
                    }
                }
            }
            return BadRequest(new { status = "fail", message = "Either movie or the image cannot be found" });


        }


        private bool MovieExists(int id)
        {
            return _db.Movies.Any(e => e.Id == id);
        }

        private async Task<List<Image>> UploadImages(List<IFormFile> files)
        {
            string folder = "images/MovieImages/";
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
                } else
                {
                    throw new IOException("File has not content");
                }
            }

            return images;
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
