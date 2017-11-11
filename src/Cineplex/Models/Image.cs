using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Cineplex.Models
{
    public class Image
    {
        public int Id { get; set; }
        [Required]
        public string FileName { get; set; }
    }
}
