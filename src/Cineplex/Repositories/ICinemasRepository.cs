using Cineplex.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cineplex.Repositories
{
    public interface ICinemasRepository
    {
        IEnumerable<Cinema> GetAll();
        Cinema Find(int Id);
        Cinema Update(Cinema cinema);
        Cinema Insert(Cinema cinema);
        IEnumerable<Movie> GetMovies(int Id);
    }
}
