using System;
using System.ComponentModel.DataAnnotations;

namespace WeddingPlanner
{
    public class FutureDate : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value == null)
            {
                return new ValidationResult("Value was null");
            }
            DateTime date = (DateTime)value;
            if (date < DateTime.Now)
            {
                return new ValidationResult("Must be in the future");
            }
            return ValidationResult.Success;
        }
    }
}