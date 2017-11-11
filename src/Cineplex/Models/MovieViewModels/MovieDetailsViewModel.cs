using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cineplex.Models.MovieViewModels
{
    public class MovieDetailsViewModel
    {
        public Movie Movie { get; set; }
        public List<Show> Shows { get; set; }
        public Cart Cart { get; set; }
        public List<PricingType> PricingTypes { get; set; }
        public List<Cinema> Cinemas { get; set; }
        public List<Rating> Ratings { get; set; }
    }
}
