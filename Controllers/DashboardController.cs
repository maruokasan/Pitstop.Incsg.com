using Pitstop.Helper;
using Pitstop.Models;
using Pitstop.Models.PitstopData;
using Pitstop.Service;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Pitstop.Models.Common;
using Microsoft.AspNetCore.Hosting;
using System.Security.Claims;


namespace Pitstop.Controllers
{
    public class DashboardController : Controller
    {
        private readonly UtilityClass _utility;
        private readonly AppKeysOption _options;
        private readonly PitstopContext _PitstopContext;
        private readonly ILogger<DashboardController> _logger;
        private readonly UserManager<User> _userManager;
        private readonly CommonService _commonService;
        private readonly IWebHostEnvironment _environment;

        public DashboardController(
            IWebHostEnvironment environment,
            ILogger<DashboardController> logger,
            PitstopContext PitstopContext,
            UserManager<User> userManager,
            UtilityClass utility,
            IOptions<AppKeysOption> options,
            CommonService commonService)
        {
            _environment = environment;
            _logger = logger;
            _PitstopContext = PitstopContext;
            _userManager = userManager;
            _commonService = commonService;
            _utility = utility;
            _options = options.Value;
        }

#region Index
        // [Authorize(ClaimType = AppConstant.RoleClaim.ClaimType.SSP, ClaimValue = AppConstant.RoleClaim.ClaimValue.Dashboard_D)]
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult GetUserData()
        {
        // Get the user's ID and role from claims
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        var userRoleClaim = User.FindFirst(ClaimTypes.Role);

        if (userIdClaim == null)
        {
            return BadRequest("User ID not found in claims");
        }

        // Extract user ID from the claim value
        var userId = userIdClaim.Value;

        // Query the database to get user details
        var user = _PitstopContext.Users
            .Where(u => u.Id == userId)
            .FirstOrDefault();

        if (user == null)
        {
            return BadRequest("User not found in the database");
        }

        // Create a UserData object with user information
        var userData = new User
        {
            UserName = user.UserName, // Replace with your actual property name
            Email = user.Email, // Replace with your actual property name
            // Add other properties as needed
        };

        return Ok(userData);
        }
#endregion

#region product
        public IActionResult Products()
        {
            return View();
        }

        public IActionResult GetDataProduct()
        {
            var productList = _PitstopContext.Product.ToList();
            var productData = productList.Select(p => new
            {
                id = p.Id, // Assuming Id is the primary key of your Product table
                name = p.Name,
                sku = p.SKU,
                qty = p.Qty,
                price = p.Price,
                status = p.Status,
                category = p.Category
            }).ToList();

            return Json(new { data = productData });
        }

        public IActionResult EditProduct(string id) // Change parameter type to string
        {
            var product = _PitstopContext.Product.FirstOrDefault(p => p.Id == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        [HttpPost]
        public IActionResult EditProduct(Product product)
        {
            if (ModelState.IsValid)
            {
                _PitstopContext.Product.Update(product);
                _PitstopContext.SaveChanges();
                return RedirectToAction("Products");
            }

            return View(product);
        }

        public IActionResult AddProduct()
        {
            return View();
        }

        [HttpPost]
        public IActionResult AddProduct(Product product)
        {
            if (ModelState.IsValid)
            {   
                _PitstopContext.Product.Add(product);
                _PitstopContext.SaveChanges();
                return RedirectToAction("Products");
            }

            return View(product);
        }   

#endregion

        public IActionResult Categories()
        {
            return View();
        }
        public IActionResult Addcategory()
        {
            return View();
        }
        public IActionResult Editcategory()
        {
            return View();
        }
        public IActionResult Orderlisting()
        {
            return View();
        }
        public IActionResult Orderdetails()
        {
            return View();
        }
        public IActionResult Addorder()
        {
            return View();
        }
        public IActionResult Editorder()
        {
            return View();
        }
        public IActionResult Customerdetails()
        {
            return View();
        }
        public IActionResult Customerlisting()
        {
            return View();
        }
        public IActionResult Customerorders()
        {
            return View();
        }
        public IActionResult Returns()
        {
            return View();
        }
        public IActionResult Shipping()
        {
            return View();
        }
        public IActionResult Productsview()
        {
            return View();
        }
        public IActionResult Sales()
        {
            return View();
        }

        #region Settings
        public IActionResult Settings()
        {
            return View();
        }
        public IActionResult Carousel1Upload(IFormFile mediaFile)
        {
            try
            {
                if (mediaFile != null && mediaFile.Length > 0)
                {
                    // Delete existing files in the folder
                    string section1MediaFolder = Path.Combine(_environment.WebRootPath, "Section1Media");
                    DirectoryInfo directory = new DirectoryInfo(section1MediaFolder);
                    foreach (FileInfo file in directory.GetFiles())
                    {
                        file.Delete();
                    }

                    // Save the new media file to the Section1Media folder
                    string uniqueFileName = Guid.NewGuid().ToString() + "_" + mediaFile.FileName;
                    string filePath = Path.Combine(section1MediaFolder, uniqueFileName);
                    mediaFile.CopyTo(new FileStream(filePath, FileMode.Create));

                    // Create a new Section1Media entry
                    var newMediaEntry = new Section1Media
                    {
                        FileName = uniqueFileName,
                        FileType = Path.GetExtension(mediaFile.FileName),
                        UploadDate = DateTime.Now
                    };

                    _PitstopContext.Section1Media.Add(newMediaEntry);
                    _PitstopContext.SaveChanges();

                    return Ok(new { success = true, message = "Media uploaded successfully" });
                }
                else
                {
                    return BadRequest("No file selected for upload.");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        public IActionResult GetCarousel1Data()
        {
            try
            {
                // Fetch the latest Section1Media data sorted by upload date in descending order
                var carouselItems = _PitstopContext.Section1Media
                    .OrderByDescending(item => item.UploadDate)
                    .Select(item => new
                    {
                        item.Id,
                        item.FileName,
                        item.FileType,
                        item.UploadDate
                    })
                    .ToList();

                return Json(carouselItems);
            }
            catch (Exception ex)
            {
                // Log and handle exceptions as needed
                Console.WriteLine("Error retrieving carousel data: " + ex.Message);
                return StatusCode(500, "An error occurred while retrieving carousel data.");
            }
        }

       public IActionResult Carousel2Upload(IFormFile imageFile, int order)
{
    try
    {
        if (imageFile != null && imageFile.Length > 0)
        {
            // Determine the carouselNumber based on the 'order'
            int carouselNumber = order;

            // Check if there is an existing entry for the specified carousel number
            var existingMedia = _PitstopContext.Section2Media.FirstOrDefault(m => m.CarouselNumber == carouselNumber);

            string uniqueFileName = Guid.NewGuid().ToString() + "_" + imageFile.FileName;
            string carousel2Folder = Path.Combine(_environment.WebRootPath, "Section2Media");
            string filePath = Path.Combine(carousel2Folder, uniqueFileName);

            // Ensure the Section2Media folder exists
            if (!Directory.Exists(carousel2Folder))
            {
                Directory.CreateDirectory(carousel2Folder);
            }

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                imageFile.CopyTo(stream);
            }

            if (existingMedia != null)
            {
                // If an existing entry exists for the carousel number, update it
                string existingFilePath = Path.Combine(carousel2Folder, existingMedia.FileName);
                if (System.IO.File.Exists(existingFilePath))
                {
                    System.IO.File.Delete(existingFilePath);
                }

                existingMedia.FileName = uniqueFileName;
                existingMedia.FileType = Path.GetExtension(imageFile.FileName); // Assuming you want to store the file type
                existingMedia.UploadDate = DateTime.Now; // You can set the upload date here
                existingMedia.Caption = "Optional caption"; // You can set the caption here
            }
            else
            {
                // If no existing entry exists, create a new one with the specified carousel number
                var newMediaEntry = new Section2Media
                {
                    FileName = uniqueFileName,
                    FileType = Path.GetExtension(imageFile.FileName), // Assuming you want to store the file type
                    UploadDate = DateTime.Now, // You can set the upload date here
                    Caption = "Optional caption", // You can set the caption here
                    CarouselNumber = carouselNumber // Set the carousel number based on the 'order'
                };

                _PitstopContext.Section2Media.Add(newMediaEntry);
            }

            _PitstopContext.SaveChanges();

            return Ok(new { success = true, message = "Image uploaded successfully", carouselNumber });
        }
        else
        {
            return BadRequest("No file selected for upload.");
        }
    }
    catch (Exception ex)
    {
        return StatusCode(500, $"Internal server error: {ex.Message}");
    }
}

        public IActionResult UploadFeaturedItem(IFormFile imageFile, string title, string price, string order)
        {
            try
            {
                if (imageFile != null && imageFile.Length > 0)
                {
                    // Check if there is an existing item with the same order
                    var existingItem = _PitstopContext.FeaturedItems.FirstOrDefault(item => item.Order == order);
                    if (existingItem != null)
                    {
                        // Delete the existing file associated with the item
                        string existingFilePath = Path.Combine(_environment.WebRootPath, "FeaturedItems", existingItem.FileName);
                        if (System.IO.File.Exists(existingFilePath))
                        {
                            System.IO.File.Delete(existingFilePath);
                        }
                        // Remove the existing item from the database
                        _PitstopContext.FeaturedItems.Remove(existingItem);
                    }

                    // Retrieve the latest Id from the database
                    string latestId = _commonService.GetLatestFeaturedItemId(); // Implement this method in your service

                    // Increment the latestId by 1 to generate the new Id
                    string newId = IncrementId(latestId); // Implement this method

                    // Save the new image file
                    string uniqueFileName = newId + "_" + imageFile.FileName;
                    string featuredItemFolder = Path.Combine(_environment.WebRootPath, "FeaturedItems");
                    string filePath = Path.Combine(featuredItemFolder, uniqueFileName);
                    if (!Directory.Exists(featuredItemFolder))
                    {
                        Directory.CreateDirectory(featuredItemFolder);
                    }
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        imageFile.CopyTo(stream);
                    }

                    // Store the file information in the FeaturedItem database table
                    var newFeaturedItem = new FeaturedItem
                    {
                        Id = newId,
                        FileName = uniqueFileName,
                        Title = title,
                        Price = price,
                        Order = order,
                        CreatedAt = DateTime.Now,
                        UpdatedAt = DateTime.Now
                    };

                    _PitstopContext.FeaturedItems.Add(newFeaturedItem);
                    _PitstopContext.SaveChanges();

                    return Ok(new { success = true, message = "Featured item uploaded successfully" });
                }
                else
                {
                    return BadRequest("No file selected for upload.");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        private string IncrementId(string latestId)
        {
            int id;
            if (int.TryParse(latestId, out id))
            {
                id++; // Increment the Id by 1
                return id.ToString().PadLeft(latestId.Length, '0'); // Pad the Id with leading zeros if necessary
            }
            else
            {
                // If the latestId cannot be parsed as an integer, handle the error or return a default value
                throw new Exception("Invalid Id format.");
            }
        }
        public IActionResult UploadProductMedia(IFormFile file, string title, string price)
        {
            try
            {
                if (file != null && file.Length > 0)
                {
                    // Check if there is an existing entry for the product media
                    var existingMedia = _PitstopContext.ProductMedia.FirstOrDefault();

                    string uniqueFileName = Guid.NewGuid().ToString() + "_" + file.FileName;
                    string productMediaFolder = Path.Combine(_environment.WebRootPath, "ProductMedia");
                    string filePath = Path.Combine(productMediaFolder, uniqueFileName);

                    // Ensure the ProductMedia folder exists
                    if (!Directory.Exists(productMediaFolder))
                    {
                        Directory.CreateDirectory(productMediaFolder);
                    }

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        file.CopyTo(stream);
                    }

                    if (existingMedia != null)
                    {
                        // If an existing entry exists, update it
                        string existingFilePath = Path.Combine(productMediaFolder, existingMedia.FileName);
                        if (System.IO.File.Exists(existingFilePath))
                        {
                            System.IO.File.Delete(existingFilePath);
                        }

                        existingMedia.FileName = uniqueFileName;
                        existingMedia.Title = title;
                        existingMedia.Price = price;
                        existingMedia.UpdatedAt = DateTime.Now;
                    }
                    else
                    {
                        // If no existing entry exists, create a new one
                        var newMediaEntry = new ProductMedia
                        {
                            FileName = uniqueFileName,
                            Title = title,
                            Price = price,
                            CreatedAt = DateTime.Now,
                            UpdatedAt = DateTime.Now
                        };

                        _PitstopContext.ProductMedia.Add(newMediaEntry);
                    }

                    _PitstopContext.SaveChanges();

                    return Ok(new { success = true, message = "Product media uploaded successfully" });
                }
                else
                {
                    return BadRequest("No file selected for upload.");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
        public IActionResult GetFeaturedItems()
        {
            try
            {
                var featuredItems = _PitstopContext.FeaturedItems.ToList();
                return Ok(featuredItems);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
        public IActionResult UploadOurStoryItem()
        {
            try
            {
                var title = Request.Form["title"];
                var description = Request.Form["description"];
                var file = Request.Form.Files["image"];

                if (file != null && file.Length > 0)
                {
                    // Process the file
                    var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                    var filePath = Path.Combine(_environment.WebRootPath, "OurStoryImages", fileName);

                    // Delete existing files in the OurStoryImages folder
                    string[] existingFiles = Directory.GetFiles(Path.Combine(_environment.WebRootPath, "OurStoryImages"));
                    foreach (var existingFile in existingFiles)
                    {
                        System.IO.File.Delete(existingFile);
                    }

                    // Save the new file
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        file.CopyTo(stream);
                    }

                    // Create a new OurStory item
                    var story = new OurStorys
                    {
                        Title = title,
                        Description = description,
                        ImageUrl = fileName,
                        CreatedAt = DateTime.Now,
                        UpdatedAt = DateTime.Now
                    };

                    // Add the new OurStory item to the context
                    _PitstopContext.OurStorys.Add(story);

                    // Save changes to the database
                    _PitstopContext.SaveChanges();

                    return Ok(new { success = true, message = "Our Story item uploaded successfully" });
                }
                else
                {
                    return BadRequest("No file selected for upload");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        #endregion
    }
}
