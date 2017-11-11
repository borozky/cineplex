using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Cineplex.Models
{
    public class Enquiry
    {
        public int Id { get; set; }
        [Required]
        [EmailAddress]
        public string From { get; set; }
        [EmailAddress]
        public string To { get; set; }
        [Required]
        public string Message { get; set; }

        public int EventId { get; set; }
        public Event Event { get; set; }
    }
}
