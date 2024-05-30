using System.ComponentModel.DataAnnotations;

namespace votingapp.Models
{
    public class Voter
    {
        [Key]
        public int id { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Name should be between 2 and 100 characters.")]
        public string name { get; set; }

        public bool has_voted { get; set; }
    }
}
