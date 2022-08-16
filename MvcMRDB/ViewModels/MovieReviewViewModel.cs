using MvcMRDB.Models;

namespace MvcMRDB.ViewModels
{
    public class MovieReviewViewModel
    {
        public Movie Movie { get; set; }

        public Review Review { get; set; }
    }
}
