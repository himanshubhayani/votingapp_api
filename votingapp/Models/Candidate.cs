using System.ComponentModel.DataAnnotations;
using Xunit.Sdk;

namespace votingapp.Models
{
    public class Candidate
    {
        [Key]
        public int id { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Name should be between 2 and 100 characters.")]
        public string name { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Votes cannot be negative.")]
        public int votes { get; set; }
    }

}
