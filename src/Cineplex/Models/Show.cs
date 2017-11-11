using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Cineplex.Models
{
    public class Show : IValidatableObject
    {
        public int Id { get; set; }

        [Display(Name ="Session Times")]
        [Required]
        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-ddTHH:mm:ssZ}", ApplyFormatInEditMode = true)]
        public DateTime SessionTime { get; set; }
        
        [Required]
        public int MovieId { get; set; }
        
        public Movie Movie { get; set; }

        [Required]
        public int CinemaId { get; set; }
        
        public Cinema Cinema { get; set; }

        public List<Booking> Bookings { get; set; }


        // for cart
        [NotMapped]
        public Dictionary<int, int> TicketQuantities { get; set; }

        // show is about to start within 30 minutes
        public bool IsAboutToStart()
        {
            return DateTime.Now.AddMinutes(30) > SessionTime;
        }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (SessionTime <= DateTime.Now)
            {
                yield return new ValidationResult("Session time for the movie show cannot be already happened.", new string[] { "SessionTime" });
            }

            if (SessionTime > DateTime.Now)
            {
                if (SessionTime < DateTime.Now.AddHours(12))
                {
                    yield return new ValidationResult("Cannot create movie shows that will happen within next 12 hours", new string[] { "SessionTime" });
                }
            }

            yield return ValidationResult.Success;
        }
    }
}
