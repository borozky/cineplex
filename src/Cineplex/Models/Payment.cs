using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Cineplex.Models
{
    // SHOULD I SAVE PAYMENT DETAILS ON THE DATABASE ???
    [NotMapped]
    public class Payment : IValidatableObject
    {
        [Required]
        [CreditCard]
        public string CreditCardNumber { get; set; }

        [Required]
        public int ExpiryYear { get; set; }

        [Required]
        [Range(1, 12)]
        public int ExpiryMonth { get; set; }

        [Required]
        public int CVC { get; set; }

        // SHOULD I INCLUDE CARD HOLDER NAME? I GUESS YOU SHOULD
        [Required]
        [MinLength(3)]
        public string CardHolder { get; set; }

        // The validations above will be the first to execute. 
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            int currentyear = DateTime.Now.Year;
            int currentmonth = DateTime.Now.Month;

            if (currentyear > ExpiryYear)
            {
                yield return new ValidationResult("Card has already been expired", new string[] { "ExpiryYear", "ExpiryMonth" });
            }

            if (currentyear == ExpiryYear && currentmonth > ExpiryMonth)
            {
                yield return new ValidationResult("Card has already been expired", new string[] { "ExpiryYear", "ExpiryMonth" });
            }

            int cvclength = CVC.ToString().Length;
            if (cvclength != 3)
            {
                yield return new ValidationResult("CVC must be exactly 3 digits long", new string[] { "CVC" });
            }

            yield return ValidationResult.Success;
            
        }
        
    }
}
