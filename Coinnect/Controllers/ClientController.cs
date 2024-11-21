using Coinnect.Data;
using Coinnect.Models.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static Coinnect.Models.AllModels;
using static Coinnect.Models.ViewModels.UserViewModels;

namespace Coinnect.Controllers
{
    public class ClientController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly CoinnectDbContext _coinnectDbContext;

        public ClientController(CoinnectDbContext coinnectDbContext, UserManager<User> userManager)
        {
            _coinnectDbContext = coinnectDbContext;
            _userManager = userManager;
        }

        [HttpGet]
        public IActionResult About()
        {
            return View(new RatingViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> About(RatingViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.GetUserAsync(User);
                var rating = new Review
                {
                    UserId = user.Id,
                    Rating = model.Rating,
                    ReviewText = model.Review,
                    DatePosted = DateTime.Now
                };

                _coinnectDbContext.Reviews.Add(rating);
                await _coinnectDbContext.SaveChangesAsync();

                return RedirectToAction("Index", "Home");
            }

            return View(model);
        }
        public async Task<IActionResult> ViewRatingsReviews()
        {
            var ratings = await _coinnectDbContext.Reviews
                                        .Include(r => r.Client)
                                        .ToListAsync();
            var reviews = new ListViewModel
            {
                Reviews = ratings
            };
            return View(reviews);
        }

    }
}
