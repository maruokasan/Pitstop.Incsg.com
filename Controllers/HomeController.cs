using Pitstop.Models;
using Pitstop.Models.PitstopData;
using Pitstop.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.Extensions.Options;
using Pitstop.Models.Common;
using Pitstop.Helper;

namespace Pitstop.Controllers
{
    public class HomeController : Controller
    {
        private readonly PitstopContext _PitstopContext;
        private readonly ILogger<HomeController> _logger;
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<Role> _roleManager;
        private readonly SignInManager<User> _signInManager;
        private readonly CommonService _commonService;
        private readonly EmailService _emailService;

        public HomeController(
          ILoggerFactory loggerFactory,
          PitstopContext PitstopContext, UserManager<User> userManager, RoleManager<Role> roleManager, SignInManager<User> signInManager,
          CommonService commonService, EmailService emailService)
        {
            _logger = loggerFactory.CreateLogger<HomeController>();
            _PitstopContext = PitstopContext;
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;
            _commonService = commonService;
            _emailService = emailService;
        }

        [AllowAnonymous]
        public async Task<IActionResult> Index()
        {
            var viewModel = new HomepageModel();

            if (User.Identity.IsAuthenticated)
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var user = await _userManager.FindByIdAsync(userId);
                var roles = await _userManager.GetRolesAsync(user);

                viewModel.UserName = user?.UserName; // Assuming the User entity has a Name property
                viewModel.UserEmail = user?.Email;
                viewModel.UserRole = roles.FirstOrDefault();
            }

            viewModel.Section1Medias = _commonService.GetSection1Media();
            viewModel.Section2Medias = _commonService.GetSection2Media(); 
            viewModel.FeaturedItems = _commonService.GetFeaturedItems();
            viewModel.ProductMedias = _commonService.GetProductMedia();
            viewModel.OurStorys = _commonService.GetOurStory();

            return View(viewModel);
        }
        public IActionResult GetCarouselImages(int carouselNumber)
        {
            try
            {
                // Implement logic to retrieve the image URLs for the specified carousel based on 'carouselNumber'

                List<string> carouselImages = new List<string>(); // Get the URLs from the database

                return Ok(new { success = true, images = carouselImages });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }


        public IActionResult GetProductMedia()
        {
            try
            {
                var featuredItems = _PitstopContext.FeaturedItems
                    .Take(6) // Limit to the first six items
                    .ToList();

                return Ok(featuredItems);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
        public OurStorys GetOurStory()
        {
            try
            {
                // Retrieve the latest OurStory item based on the UpdatedAt property
                var latestStory = _PitstopContext.OurStorys
                    .OrderByDescending(s => s.UpdatedAt)
                    .FirstOrDefault();

                return latestStory;
            }
            catch (Exception ex)
            {
                // Log or handle the exception as needed
                throw ex;
            }
        }

        public IActionResult Cart()
        {
            return View();
        }
        public IActionResult AddToCart(string productId, string quantity, string size)
        {
            string sessionId = HttpContext.Session.Id; // Get the session ID

            // Check if the product is already in the cart for the current session
            var existingCartItem = _PitstopContext.Carts.FirstOrDefault(c => c.SessionId == sessionId && c.ProductId == productId && c.Size == size);

            if (existingCartItem != null)
            {
                // Update quantity if the product already exists in the cart
                existingCartItem.Quantity += quantity;
            }
            else
            {
                // Add a new cart item if it doesn't exist in the cart
                var newCartItem = new Cart
                {
                    SessionId = sessionId,
                    ProductId = productId.ToString(), // Assuming ProductId is a string in the Cart class
                    Quantity = quantity.ToString(), // Assuming Quantity is a string in the Cart class
                    Size = size,
                    CreateAt = DateTime.Now // Set the creation date/time
                };
                _PitstopContext.Carts.Add(newCartItem);
            }

            _PitstopContext.SaveChanges();

            return RedirectToAction("Cart");
        }


    }
}
