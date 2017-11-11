using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Cineplex.Models
{
    public class Cinema
    {
        public Cinema(){}

        [Key]
        public int Id { get; set; }

        [Required]
        public string Location { get; set; }

        [Required]
        public string Details { get; set; }

        [Required]
        public int Seats { get; set; }

        public List<Show> Shows { get; set; }

        public List<Image> Images { get; set; }
    }
}
