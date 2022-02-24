using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WeddingPlanner.Models
{
    public class Wedding
    {
        [Key]
        public int WeddingId { get; set; }

        [Required]
        [MinLength(2, ErrorMessage = "must be at least 2 characters.")]
        public string ToBeNameOne { get; set; }

        [Required]
        [MinLength(2, ErrorMessage = "must be at least 2 characters.")]
        public string ToBeNameTwo { get; set; }

        [Display(Name = "Wedding Date")]
        [DataType(DataType.Date)]
        [FutureDate] // See CustomValidators.cs
        public DateTime? Date { get; set; } // Future only validation
        public string Address { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;

        public int UserId { get; set; }

        public List<RSVP> RSVPs { get; set; }
    }
}