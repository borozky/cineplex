using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Cineplex.Models
{
    public class Booking : IValidatableObject
    {
        public int Id { get; set; }

        public int ShowId { get; set; }
        
        public Show Show { get; set; }
        
        public int OrderId { get; set; }
        public Order Order { get; set; }
        
        
        public List<Ticket> Tickets { get; set; }
        
        public List<Seat> Seats { get; set; }

        public double CalculateTotal()
        {
            double total = 0.0;
            if (Tickets != null && Tickets.Any())
            {
                Tickets.ForEach(t => {
                    total += (double)(t.Quantity * t.PricingType.Value);
                });
            }
            return total;
        }

        public int TotalNumTickets()
        {
            int total = 0;
            if (Tickets != null)
            {
                Tickets.ForEach(t => { total += t.Quantity; });
            }
            return total;
        }

        public bool HasRemainingSeatsLeft()
        {
            int totalTickets = 0;
            if (Tickets != null)
            {
                Tickets.ForEach(t => totalTickets += t.Quantity);
            }

            try
            {
                if (Seats.Count() >= totalTickets)
                {
                    return false;
                } else
                {
                    return true;
                }
            }
            catch (Exception)
            {
                return true;
            }



        }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (TotalNumTickets() <= 0)
            {
                yield return new ValidationResult("Please enter number of tickets above 0");
            }

            yield return ValidationResult.Success;
        }
    }
}
