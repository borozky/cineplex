using Cineplex.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cineplex.Models.HomeViewModels
{
    public class HomeIndexViewModel
    {
        public List<Movie> Movies { get; set; }
        public List <Cinema> Cinemas { get; set; }
        public List <Show> Shows { get; set; }
        public List<Event> Events { get; set; }
    }
}
