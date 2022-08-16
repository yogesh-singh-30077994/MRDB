using MvcMRDB.Areas.Identity.Data;
using System.ComponentModel.DataAnnotations;

namespace MvcMRDB.Models
{
    public class Review
    {
        public int ReviewId { get; set; }

        [Required]
        [StringLength(180)]
        public string Content { get; set; }

        [Required]
        public int MovieId { get; set; }
        public Movie Movie { get; set; }

        public string ApplicationUserId { get; set; }
        public ApplicationUser ApplicationUser { get; set; }
    }
}
