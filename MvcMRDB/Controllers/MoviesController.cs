using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MvcMRDB.Areas.Identity.Data;
using MvcMRDB.Data;
using MvcMRDB.Models;
using MvcMRDB.ViewModels;

namespace MvcMRDB.Controllers
{
    public class MoviesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        
        public MoviesController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index(string searchString, string searchBy="Title")
        {
            var movies = from m in _context.Movie
                         select m;

            if (!string.IsNullOrEmpty(searchString))
            {
                if (searchBy == "Title")
                {
                    movies = from m in _context.Movie
                             where m.Title!.Contains(searchString)
                             select m;

                } 
                else if (searchBy == "Genre")
                {
                    movies = from m in _context.Movie
                             where m.Genre!.Contains(searchString)
                             select m;
                }
            }
            return View(await movies.ToListAsync());
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var movie = await _context.Movie
                .Include(m => m.Reviews)
                .FirstOrDefaultAsync(m => m.MovieId == id);

            // find better way
            foreach (var review in movie.Reviews)
            {
                var user = await _userManager.FindByIdAsync(review.ApplicationUserId);
                review.ApplicationUser = user;
            }

            if (movie == null)
            {
                return NotFound();
            }

            var viewModel = new MovieReviewViewModel
            {
                Movie = movie
            };

            var authenticatedUser = await _userManager.GetUserAsync(HttpContext.User);
            ViewData["authenticatedUserId"] = authenticatedUser.Id;

            return View(viewModel);
        }

        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            return View();
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("MovieId,Title,ReleaseDate,Genre")] Movie movie)
        {
            if (ModelState.IsValid)
            {
                _context.Add(movie);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(movie);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddReview([Bind("ReviewId,Content,MovieId")] Review review)
        {
            if (ModelState.IsValid)
            {
                var authenticatedUser = await _userManager.GetUserAsync(HttpContext.User);
                review.ApplicationUserId = authenticatedUser.Id;
                //review.User = authenticatedUser; does not work
                _context.Add(review);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Details), new {id = review.MovieId});
            }

            var viewModel = new MovieReviewViewModel
            {
                Review = review,
                Movie = await _context.Movie
                .Include(m => m.Reviews)
                .FirstOrDefaultAsync(m => m.MovieId == review.Movie.MovieId)
            };

            return View("Details", viewModel);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Movie == null)
            {
                return NotFound();
            }

            var movie = await _context.Movie.FindAsync(id);
            if (movie == null)
            {
                return NotFound();
            }
            return View(movie);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("MovieId,Title,ReleaseDate,Genre")] Movie movie)
        {
            if (id != movie.MovieId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(movie);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Movie.Any(m => m.MovieId == id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(movie);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            if (_context.Movie == null)
            {
                return Problem("Entity set 'MovieContext.Movie'  is null.");
            }
            var movie = await _context.Movie.FindAsync(id);
            if (movie != null)
            {
                _context.Movie.Remove(movie);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
