using Pitstop.Models;
using Pitstop.Models.PitstopData;
using Pitstop.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Pitstop.Models.Common;
using Pitstop.Helper;

namespace Pitstop.Controllers
{
    [AllowAnonymous]
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

#region Index
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
            viewModel.FeaturedItems = _commonService.GetFeaturedItems(); // Fetch only the latest 3 items
            viewModel.ProductMedias = _commonService.GetProductMedia();
            viewModel.OurStorys = _commonService.GetOurStory();
            viewModel.Testimonial = _commonService.GetTestimonials();

            return View(viewModel);
        }
        
        [AllowAnonymous]
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
        
        [AllowAnonymous]
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
       [
        AllowAnonymous]
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

        [AllowAnonymous]
        public IActionResult GetTestimonials()
        {
            try
            {
                var testimonials = _PitstopContext.Testimonial.ToList(); // Use the correct DbSet property name

                return Ok(testimonials);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

#endregion

#region Cart
        public IActionResult Cart()
        {
            // Retrieve cart items from the database for the current user's session
            var sessionId = HttpContext.Session.Id;
            var cartItems = _PitstopContext.Carts
                .Where(c => c.SessionId == sessionId)
                .ToList();

            // Process each cart item to include product details and split sizes
            foreach (var cartItem in cartItems)
            {
                // Fetch associated product details
                var product = _PitstopContext.Product.FirstOrDefault(p => p.Id == cartItem.ProductId);
                if (product != null && !string.IsNullOrEmpty(product.Size))
                {
                    // Split the size string into individual sizes
                    var sizes = product.Size.Split(',').ToList();
                    // Assign the list of sizes to the AvailableSizes property of the cart item
                    cartItem.AvailableSizes = sizes;
                }
                else
                {
                    // Handle case where no sizes are available
                    cartItem.AvailableSizes = new List<string>(); // Empty list
                }
            }

            return View(cartItems);
        }
      
        [AllowAnonymous]
        public IActionResult AddToCart(string productId)
        {
            try
            {
                // Validate incoming data
                if (string.IsNullOrEmpty(productId))
                {
                    return BadRequest("Product ID is required.");
                }

                string sessionId = HttpContext.Request.Cookies[".AspNetCore.Session"]; // Retrieve the session ID from the ASP.NET Core session cookie

                string userId = GetUserIdentifier(); // Retrieve user ID
                if (string.IsNullOrEmpty(userId))
                {
                    // Set user ID to 0 for anonymous users
                    userId = "0";
                }

                // Retrieve the product from the database
                var product = _PitstopContext.Product.FirstOrDefault(p => p.Id == productId);
                if (product == null)
                {
                    // Handle if product is not found
                    return NotFound("Product not found.");
                }

                decimal discount;
                decimal tax;
                decimal.TryParse(product.Discount, out discount);
                decimal.TryParse(product.Tax, out tax);
                decimal totalPrice = decimal.Parse(product.Price) * 1;

                // Create a new cart item
                var newCartItem = new Cart
                {
                    SessionId = sessionId,
                    UserId = userId,
                    ProductId = product.Id,
                    Quantity = 1,
                    Size = product.Size,
                    TotalPrice = totalPrice,
                    Discount = discount,
                    Tax = tax,
                    Status = "processing",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    ShippingMethod = "none",
                    ShippingAddress = "none"
                };

                // Add the new cart item to the context
                _PitstopContext.Carts.Add(newCartItem);

                // Save changes to persist the new cart item
                _PitstopContext.SaveChanges();

                // Return a JSON response indicating success
                return Ok(new { success = true });
            }
            catch (Exception ex)
            {
                // Log the error and return an error response
                Console.WriteLine($"Error adding item to cart: {ex}");

                // Get the inner exception message
                string innerExceptionMessage = ex.InnerException != null ? ex.InnerException.Message : "No inner exception";

                return StatusCode(500, new { success = false, errorMessage = $"An error occurred while adding the item to the cart. Inner Exception: {innerExceptionMessage}" });
            }
        }


        [AllowAnonymous]
        private string GetUserIdentifier()
        {
            // Implement logic to retrieve user ID based on your authentication mechanism
            // For example, if you're using ASP.NET Core Identity:
            return User.FindFirstValue(ClaimTypes.NameIdentifier);
        }

        [AllowAnonymous]
        public IActionResult RemoveFromCart(int cartItemId)
        {
            try
            {
                var cartItem = _PitstopContext.Carts.Find(cartItemId);
                if (cartItem != null)
                {
                    _PitstopContext.Carts.Remove(cartItem);
                    _PitstopContext.SaveChanges();
                }

                return RedirectToAction("Cart");
            }
            catch (Exception ex)
            {
                // Log the error and return an error response
                Console.WriteLine($"Error removing item from cart: {ex.Message}");
                return StatusCode(500, "An error occurred while removing the item from the cart.");
            }
        }

        [AllowAnonymous]
        public IActionResult ClearCart()
        {
            try
            {
                var sessionId = HttpContext.Session.Id;
                var cartItems = _PitstopContext.Carts.Where(c => c.SessionId == sessionId).ToList();
                _PitstopContext.Carts.RemoveRange(cartItems);
                _PitstopContext.SaveChanges();

                // Return a success JSON response
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                // Log the error and return an error JSON response
                Console.WriteLine($"Error clearing cart: {ex.Message}");
                return Json(new { success = false, errorMessage = ex.Message });
            }
        }
  
        [AllowAnonymous]
        public IActionResult GetCartItems()
        {
            // Retrieve user identifier from cookies
            string userIdFromCookie = HttpContext.Request.Cookies[AppConstant.CookiesName.UserId];
            string sessionIdFromCookie = HttpContext.Request.Cookies[".AspNetCore.Session"];

            if (!string.IsNullOrEmpty(userIdFromCookie))
            {
                var cartItems = _PitstopContext.Carts
                    .Include(c => c.Product)
                    .Where(c => c.UserId == userIdFromCookie)
                    .ToList();

                // Split the Size string into individual sizes for each product
                foreach (var item in cartItems)
                {
                    if (!string.IsNullOrEmpty(item.Product.Size))
                    {
                        item.Product.AvailableSizes = item.Product.Size.Split(',').ToList();
                    }
                    else
                    {
                        item.Product.AvailableSizes = new List<string>();
                    }
                }

                return PartialView("_CartItems", cartItems);
            }
            else if (!string.IsNullOrEmpty(sessionIdFromCookie))
            {
                var cartItems = _PitstopContext.Carts
                    .Include(c => c.Product)
                    .Where(c => c.SessionId == sessionIdFromCookie && c.UserId == "0")
                    .ToList();

                // Split the Size string into individual sizes for each product
                foreach (var item in cartItems)
                {
                    if (!string.IsNullOrEmpty(item.Product.Size))
                    {
                        item.Product.AvailableSizes = item.Product.Size.Split(',').ToList();
                    }
                    else
                    {
                        item.Product.AvailableSizes = new List<string>();
                    }
                }

                return PartialView("_CartItems", cartItems);
            }
            else
            {
                return RedirectToAction("Login");
            }
        }


#endregion

#region Checkout
// public IActionResult Checkout(Checkout model)
//         {
//             if (ModelState.IsValid)
//             {
//                 // Process the checkout logic here
//                 // Calculate total price
//                 decimal totalPrice = CalculateTotalPrice(model.CartItems, model.DiscountCode);

//                 // Create order record in the database
//                 Order order = new Order
//                 {
//                     FullName = model.FullName,
//                     // Add more fields as needed (contact number, email, address)
//                     ShippingMethod = model.ShippingMethod,
//                     TotalPrice = totalPrice,
//                     CreatedAt = DateTime.UtcNow
//                 };

//                 _PitstopContext.Orders.Add(order);
//                 _PitstopContext.SaveChanges();

//                 // Redirect to a thank you page or order summary page
//                 return RedirectToAction("ThankYou");
//             }

//             // If ModelState is not valid, return back to the checkout page with errors
//             return View("Cart", model.CartItems);
//         }

        // Helper method to calculate total price
        private decimal CalculateTotalPrice(List<Cart> cartItems, string discountCode)
        {
            // Calculate total price based on cart items and apply discount code if applicable
            // Implement your logic here
            return 0; // Placeholder, replace with your actual calculation
        }

        // GET: ThankYou
        public IActionResult ThankYou()
        {
            // Display a thank you message or order summary page
            return View();
        }

#endregion

    }
}
