using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cineplex.Models.OrderViewModels
{
    public class OrderDetailsViewModel
    {
        public Order Order { get; set; }
        public DateTime DateCreated { get; set; }
    }
}
