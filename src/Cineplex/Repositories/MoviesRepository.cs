using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cineplex.Models;
using Cineplex.Data;
using Microsoft.EntityFrameworkCore;

namespace Cineplex.Repositories
{
    public class MoviesRepository : IMoviesRepository
    {
        public ApplicationDbContext _db;
        public MoviesRepository(ApplicationDbContext context)
        {
            _db = context;
        }

        public IEnumerable<Movie> All()
        {
            return _db.Movies
                .Include(m => m.Shows)
                    .ThenInclude(s => s.Cinema)
                .Include(m => m.Images)
                .ToList();
        }

        public Movie Find(int Id)
        {
            return _db.Movies
                .Include(m => m.Shows)
                    .ThenInclude(s => s.Cinema)
                .Include(m => m.Images)
                .Include(m => m.Rating)
                .FirstOrDefault(m => m.Id == Id);
        }

        public Movie Create(Movie movie)
        {
            var exists = _db.Movies.FirstOrDefault(m => m.Id == movie.Id);
            if (exists == null)
            {
                _db.Movies.Add(movie);
                _db.SaveChanges();
                return movie;
            }
            return null;
        }

        public IEnumerable<Movie> SearchByCinema(string location)
        {
            var found = _db.Cinemas.FirstOrDefault(c => c.Location == location);

            if (found != null)
            {
                var movieIds = _db.Shows.Where(s => s.CinemaId == found.Id && s.SessionTime > DateTime.Now)
                    .Select(s => s.MovieId)
                    .Distinct().ToArray();

                var movies = _db.Movies
                    .Include(m => m.Shows)
                        .ThenInclude(s => s.Cinema)
                    .Include(m => m.Images)
                    .Include(m => m.Rating)
                    .Where(m => movieIds.Contains(m.Id))
                    .ToList();

                return movies;
            }
            return null;
        }

        public IEnumerable<Movie> SearchByCinema(int CinemaId)
        {
            var cinema = _db.Cinemas.FirstOrDefault(c => c.Id == CinemaId);
            if (cinema != null)
            {
                var movies = _db.Shows
                            .Where(s => s.CinemaId == CinemaId && s.SessionTime > DateTime.Now)
                            .Select(s => s.Movie)
                            .Distinct()
                            .Include(m => m.Shows)
                                .ThenInclude(s => s.Cinema)
                            .Include(m => m.Images)
                            .Include(m => m.Rating)
                            .ToList();

                return movies;
                
            }
            return null;
        }

        public IEnumerable<Movie> SearchByDate(DateTime From, DateTime After)
        {

            var movies = _db.Shows
                        .Where(s => s.SessionTime.Date >= From.Date && s.SessionTime.Date <= After.Date)
                        .Select(s => s.Movie)
                        .Distinct()
                        .Include(m => m.Shows)
                        .Include(m => m.Images)
                        .Include(m => m.Rating)
                        .ToList();

            return movies;
        }

        public IEnumerable<Movie> SearchByTitle(string keyword)
        {
            if (keyword == null)
            {
                keyword = "";
            }
            var keywordLowerCasePacked = keyword.Replace(" ", "").ToLower();
            var movies = _db.Movies
                .Include(m => m.Shows)
                    .ThenInclude(s => s.Cinema)
                .Include(m => m.Images)
                .Include(m => m.Rating)
                .Where(m => m.Title.Replace(" ", "").ToLower().Contains(keywordLowerCasePacked));

            return movies != null ? movies.ToList() : null;
        }

        public Movie Update(Movie movie)
        {
            var originalMovie = _db.Movies.FirstOrDefault(m => m.Id == movie.Id);
            originalMovie.Rating = movie.Rating;
            originalMovie.Title = movie.Title;
            originalMovie.Description = movie.Description;
            originalMovie.Duration = movie.Duration;
            _db.SaveChanges();
            return movie;
        }
        public bool Delete(int MovieId)
        {
            var movie = _db.Movies.FirstOrDefault(m => m.Id == MovieId);
            if (movie != null)
            {
                _db.Movies.Remove(movie);
                _db.SaveChanges();
            }

            return (_db.Movies.FirstOrDefault(m => m.Id == MovieId) == null);
        }

    }
}
