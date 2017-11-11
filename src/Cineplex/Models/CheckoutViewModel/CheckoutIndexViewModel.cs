using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cineplex.Models.CheckoutViewModel
{
    public class CheckoutIndexViewModel
    {
        public Cart Cart { get; set; }
        public List<Movie> Movies { get; set; }
        public List<Show> Shows { get; set; }
        public List<Cinema> Cinemas { get; set; }
        public List<Rating> Ratings { get; set; }
        public ApplicationUser User { get; set; }
        public Payment Payment { get; set; }
    }
}
