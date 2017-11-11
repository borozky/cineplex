using Cineplex.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Cineplex.Models
{
    public class Order
    {
        public int Id { get; set; }

        [Required]
        public decimal Total { get; set; }

        // IdentityUser's PK is of type 'string'
        public string OrderedById { get; set; }
        [ForeignKey("OrderedById")]
        public ApplicationUser OrderedBy { get; set; }

        [Required]
        [Range(1,5)]
        public List<Booking> Bookings { get; set; }
    }
}
