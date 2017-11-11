using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Cineplex.Models
{
    public class Movie : IValidatableObject
    {
        public int Id { get; set; }

        // TITLE IS REQUIRED
        [Required]
        [StringLength(256)]
        public string Title { get; set; }

        // DESCRIPTION IS REQUIRED
        [Required]
        [DataType(DataType.Html)]
        public string Description { get; set; }
        
        public int? RatingId { get; set; }
        public Rating Rating { get; set; }
        public int Duration { get; set; }
        public List<Image> Images { get; set; }
        public List<Show> Shows { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (Duration < 0)
            {
                yield return new ValidationResult("Duration cannot be negative", new string[] { "Duration" });
            }

            yield return ValidationResult.Success;
        }


    }
}
