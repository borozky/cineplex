using Cineplex.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cineplex.Repositories
{
    public interface IMoviesRepository
    {
        IEnumerable<Movie> All();
        IEnumerable<Movie> SearchByTitle(string keyword);
        IEnumerable<Movie> SearchByCinema(int CinemaId);
        IEnumerable<Movie> SearchByCinema(string CinemaLocation);

        IEnumerable<Movie> SearchByDate(DateTime From, DateTime After);
        Movie Find(int Id);
        Movie Create(Movie movie);
        Movie Update(Movie movie);
        bool Delete(int MovieId);


    }
}
