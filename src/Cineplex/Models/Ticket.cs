using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Cineplex.Models
{
    public class Ticket
    {
        [Key]
        public long Number { get; set; }

        [Required]
        public int PricingTypeId { get; set; }
        public PricingType PricingType { get; set; }

        // THIS IS JUST TO DETERMINE NUMBER OF TICKETS
        // OF SAME PRICING TYPE SAVE INTO CART
        [NotMapped]
        [Range(0, 5)]
        public int Quantity { get; set; }
    }
}
