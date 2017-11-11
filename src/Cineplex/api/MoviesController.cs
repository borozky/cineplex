using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Cineplex.Data;
using Cineplex.Models;
using Microsoft.EntityFrameworkCore;
using Cineplex.Repositories;

// For more information on enabling Web API for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace Cineplex.api
{
    [Route("api/[controller]")]
    public class MoviesController : Controller
    {
        private IMoviesRepository repo;
        private ApplicationDbContext _db;

        public MoviesController(ApplicationDbContext db, IMoviesRepository repository)
        {
            _db = db;
            repo = repository;
            
        }

        // GET: api/Movies/
        [HttpGet]
        public IActionResult Get()
        {
            var movies = repo.All();

            return Ok(
                new
                {
                    status= "OK",
                    message= $"Found {movies.Count()} result(s)",
                    data = new { movies = movies}
                }    
            );
        }

        [HttpGet("/simple/")]
        public async Task<IActionResult> GetMovies()
        {
            var movies = await _db.Movies.ToListAsync();
            return Ok(new { status = "success" , message = $"Found {movies.Count()} results", data = new { movies = movies } });
        }
        
        
        [HttpGet("search/{criteria}/{keyword}")]
        public IActionResult Search(string criteria = null, string keyword = "")
        {
            if (criteria == null)
            {
                return NotFound(
                new
                {
                    status = "fail",
                    data = new { criteria = "Please enter a criteria (title, date, cinema)" }
                }
                );
            }

            criteria = criteria.ToLower();

            var movies = new List<Movie>();

            switch (criteria)
            {
                case "cinema":
                    var moviesByCinema = repo.SearchByCinema(keyword);
                    return Ok(
                        new
                        {
                            status = "success",
                            message = moviesByCinema != null ? $"Found {moviesByCinema.Count()} results" : "Found no results",
                            data = new { movies = moviesByCinema }
                        });
                case "title":
                    var moviesByTitle = repo.SearchByTitle(keyword);
                    return Ok(
                        new
                        {
                            status = "success",
                            message = moviesByTitle != null ? $"Found {moviesByTitle.Count()} results" : "Found no results",
                            data = new { movies = moviesByTitle }
                        });
                case "date":
                    var moviesByDate = new List<Movie>();
                    try
                    {
                        var dates = keyword.Split('|');
                        var from = Convert.ToDateTime(dates[0]);
                        var to = Convert.ToDateTime(dates[1]);

                        moviesByDate = repo.SearchByDate(from, to).ToList();
                        return Ok(
                            new
                            {
                                status = "success",
                                message = moviesByDate != null ? $"Found { moviesByDate.Count() }" : "Found no results",
                                data = new { movies = moviesByDate }
                            }
                        );
                    }
                    catch (Exception e)
                    {
                        return NotFound(
                        new
                        {
                            status = "error",
                            message = e.Message
                        });
                    }
                default:
                    return NotFound(
                      new
                      {
                          status = "fail",
                          data = new { criteria = "criteria not found" }
                      }
                );
            }
        }

        // GET api/Find/5
        [HttpGet("{id}")]
        public IActionResult Find(int id)
        {
            var movie = repo.Find(id);
            return Ok(new
            {
                status = "success",
                message = movie != null ? "Found 1 result" : "Found 0 results",
                data = new { movie = movie } 
            });
        }
        

        // POST api/movie
        [HttpPost]
        public IActionResult Post([FromBody] Movie movie)
        {
            if (movie == null)
            {
                return NotFound( new { status = "fail", message = "Movie information must be supplied" });
            }

            if (ModelState.IsValid)
            {
                var createdMovie = repo.Create(movie);
                if (movie != null)
                {
                    return Ok(new { status = "success", message = "Movie has been created", data = new { movie = movie }  });
                }
            }

            return NotFound( new { status = "fail", message = "Cannot save movie", data = ModelState.Values.Select(v => v.Errors) });
        }

        // PUT api/movie/5
        [HttpPut("{id}")]
        public IActionResult Put(int id, [FromBody] Movie movie)
        {
            if (movie == null)
            {
                return NotFound(new { status = "fail", data = new { movie = "movie is required" } });
            }

            if (movie.Id != id)
            {
                return NotFound( new { status = "fail", data = new { movie = "Movie must match with the ID supplied" } });
            }

            if (ModelState.IsValid)
            {
                var updatedMovie = repo.Update(movie);
                if (updatedMovie != null)
                {
                    return Ok(
                        new {
                            status = "OK",
                            message = "Movie has been updated",
                            data =  new { movie = updatedMovie }
                        }
                    );
                }
            }
            return NotFound(new { status = "fail", data = ModelState.Values.Select(v => v.Errors) });
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var deleted = repo.Delete(id);
            if (deleted)
            {
                return Ok(new { status = "success", message = "Movie has been deleted" });
            }

            return NotFound(new { status = "fail", message = "Cannot deleted movie" });
        }
    }
}
