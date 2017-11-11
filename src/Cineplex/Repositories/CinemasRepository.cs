using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cineplex.Models;
using Cineplex.Data;
using Microsoft.EntityFrameworkCore;

namespace Cineplex.Repositories
{
    public class CinemasRepository : ICinemasRepository
    {
        // CINEMAS CANNOT BE DESTROYED
        // THEY CAN BE ADDED BUT IT IS A RARE OCCASSION

        private ApplicationDbContext _db;

        public CinemasRepository(ApplicationDbContext context)
        {
            _db = context;
        }

        public IEnumerable<Cinema> GetAll()
        {
            return _db.Cinemas
                .Include(c => c.Images)
                .ToList();
        }

        public Cinema Find(int Id)
        {
            return _db.Cinemas
                .Include(c => c.Images)
                .FirstOrDefault(c => c.Id == Id);
        }
        

        public Cinema Update(Cinema cinema)
        {
            var originalCinema = _db.Cinemas.FirstOrDefault(c => c.Id == cinema.Id);
            
            // CINEMA LOCATION CANNOT BE UPDATED!

            originalCinema.Seats = cinema.Seats;
            _db.SaveChanges();
            return cinema;
        }

        public Cinema Insert(Cinema cinema)
        {
            var exists = _db.Movies.FirstOrDefault(c => c.Id == cinema.Id);
            if (exists == null)
            {
                _db.Cinemas.Add(cinema);
                _db.SaveChanges();
                return cinema;
            }
            return null;
        }

        public IEnumerable<Movie> GetMovies(int Id)
        {
            var movieIds = _db.Shows
                            .Where(s => s.CinemaId == Id && s.SessionTime > DateTime.Now)
                            .Select(s => s.MovieId).ToArray();

            var movies = _db.Movies
                .Include(m => m.Images)
                .Include(m => m.Shows)
                    .ThenInclude(s => s.Cinema)
                .Where(m => movieIds.Contains(m.Id))
                .ToList();

            return movies;

        }
    }
}
