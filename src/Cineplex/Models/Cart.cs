using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Cineplex.Models
{
    public class Cart : IValidatableObject
    {
        public Cart()
        {
            // ENSURE CART IS NOT NULL (IS THIS CHEATING ???)
            if (Bookings == null)
            {
                Bookings = new List<Booking>();
            }
            if (Shows == null)
            {
                Shows = new List<Show>();
            }
        }

        //public List<Booking> Bookings { get; set; }
        public List<Show> Shows { get; set; }

        public List<Booking> Bookings { get; set; }
        
        public double CalculateTotal()
        {
            double total = 0.0;
            if (Bookings != null && Bookings.Any())
            {
                Bookings.ForEach(b => total += b.CalculateTotal());
            }
            return total;
        }

        public int TotalNumTickets()
        {
            int total = 0;
            Bookings.ForEach(b => total += b.TotalNumTickets());
            return total;
        }
        public bool HasReachedBookingLimit()
        {
            return TotalNumTickets() >= 5;
        }

        public bool HasRemainingSeatsLeft()
        {
            foreach (Booking booking in Bookings)
            {
                if (booking.HasRemainingSeatsLeft())
                {
                    return true;
                }
            }

            return false;
        }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (TotalNumTickets() > 5)
            {
                yield return new ValidationResult("Number of tickets cannot be more than 5");
            }
            else if (TotalNumTickets() < 0)
            {
                yield return new ValidationResult("Number of tickets cannot be negative");
            }

            yield return ValidationResult.Success;
        }
    }
}
