using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Cineplex.Models.CustomValidators
{
    public class TicketRangeAttribute : ValidationAttribute
    {
        private List<Ticket> _tickets;
        public TicketRangeAttribute(List<Ticket> tickets)
        {
            _tickets = tickets;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            int total = 0;
            _tickets.ForEach(t => { total += t.Quantity; });
            if (total > 5)
            {
                return new ValidationResult("Number of tickets cannot be more than 5");
            } else if (total < 0)
            {
                return new ValidationResult("Number of tickets cannot be negative");
            }

            return ValidationResult.Success;
        }
    }
}
