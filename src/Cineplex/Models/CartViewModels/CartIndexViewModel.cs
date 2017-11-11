using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cineplex.Models.CartViewModels
{
    public class CartIndexViewModel
    {
        public ApplicationUser CurrentUser { get; set; }
        public List<PricingType> PricingTypes { get; set; }
        public Cart Cart { get; set; }
        public List<Movie> Movies { get; set; }
    }
}
