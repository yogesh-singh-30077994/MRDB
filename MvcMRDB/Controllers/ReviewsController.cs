using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MvcMRDB.Areas.Identity.Data;
using MvcMRDB.Data;

namespace MvcMRDB.Controllers
{
    public class ReviewsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<ReviewsController> _logger;

        public ReviewsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, ILogger<ReviewsController> logger)
        {
            _context = context;
            _userManager = userManager;
            _logger = logger;
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return BadRequest();

            var review = await _context.Review.FindAsync(id);
            if (review == null)
            {
                return NotFound();
            }

            var authenticatedUser = await _userManager.GetUserAsync(HttpContext.User);
            if (review.ApplicationUserId == authenticatedUser.Id ||
                HttpContext.User.IsInRole("Admin"))
            {
                _context.Remove(review);
                await _context.SaveChangesAsync();

                return RedirectToAction("Details", "Movies", new { id = review.MovieId });
            }
            
            return BadRequest();
        }
    }
}
