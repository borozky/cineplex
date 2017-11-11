using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cineplex.Models.CinemaViewModels
{
    public class CinemaMoviesViewModel
    {
        public List<Movie> Movies { get; set; }
        public Cinema Cinema { get; set; }
    }
}
