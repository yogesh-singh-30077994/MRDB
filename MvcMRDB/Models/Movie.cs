using System.ComponentModel.DataAnnotations;

namespace MvcMRDB.Models
{
    public class Movie
    {
        public int MovieId { get; set; }

        [Required]
        public string Title { get; set; }

        [Required]
        [Display(Name = "Release Date")]
        [DataType(DataType.Date)]
        public DateTime ReleaseDate { get; set; }

        [Required]
        public string Genre { get; set; }

        public ICollection<Review> Reviews { get; set; }
    }
}
