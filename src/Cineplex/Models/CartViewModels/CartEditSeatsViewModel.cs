using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cineplex.Models.CartViewModels
{
    public class CartEditSeatsViewModel
    {
        public Booking Booking { get; set; }
        public int[] SeatsUsed { get; set; }
        public int[] Reserved { get; set; }
        public Show Show { get; set; }
    }
}
