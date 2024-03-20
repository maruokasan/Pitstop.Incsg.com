using Pitstop.Helper;
using Pitstop.Models;
using Pitstop.Models.PitstopData;
using Pitstop.Service;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Pitstop.Models.Common;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;



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
    var products = _PitstopContext.Product.ToList();

    // Calculate total quantity of products
    long totalQuantity = products.Sum(p => string.IsNullOrEmpty(p.Qty) ? 0 : long.Parse(p.Qty));

    // Fetch categories from the database
    var categories = _PitstopContext.Category.ToList();

    var statuses = _PitstopContext.Product
        .Select(p => p.Status)
        .Distinct()
        .ToList();

    // Create a DashboardViewModel instance and populate its properties
    var viewModel = new DashboardViewModel
    {
        Products = products,
        TotalQuantity = totalQuantity,
        Categorys = categories, // Assign categories to the Categories property
        Statuses = statuses // Assign statuses to the Statuses property
    };

    // Pass the viewModel to the view
    return View(viewModel);
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

#region Product
        public IActionResult Products()
        {
            // Fetch categories from the database
            var categories = _PitstopContext.Category.ToList();

            // Pass categories to the view
            ViewBag.Categories = new SelectList(categories, "Id", "Name");

            return View();
        }

        public IActionResult GetDataProduct()
        {
            var productList = _PitstopContext.Product.ToList();
            var productData = productList.Select(p => new
            {
                id = p.Id,
                name = p.Name,
                sku = p.SKU,
                size = p.Size,
                qty = p.Qty,
                price = p.Price,
                status = p.Status,
                category = p.Category,
                thumbnail = p.Thumbnail,
                description = p.Description,
                tags = p.Tags,
                discount = p.Discount,
                tax = p.Tax,
                vat = p.Vat,
                barcode = p.Barcode,
                media = p.Media,

            }).ToList();

            return Json(new { data = productData });
        }
       public async Task<IActionResult> EditProduct(Product product, string[] sizes, IFormFile? thumbnail)
{
    if (!ModelState.IsValid)
    {
        // Log ModelState errors
        foreach (var key in ModelState.Keys)
        {
            foreach (var error in ModelState[key].Errors)
            {
                Console.WriteLine($"Field: {key}, Error: {error.ErrorMessage}");
            }
        }

        // Return error response with ModelState errors
        return Json(new { success = false, message = "Invalid model state", errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage) });
    }

    try
    {
        // Concatenate available sizes into a single string
        string availableSizes = string.Join(",", product.AvailableSizes);

        // Update the product with the new data
        _PitstopContext.Update(product);

        // Assign the concatenated available sizes to the Size property
        product.Size = availableSizes;

        // Check if a new thumbnail file is uploaded
        if (thumbnail != null && thumbnail.Length > 0)
        {
            // Process the new thumbnail file
            string newThumbnailFileName = await SaveThumbnailAsync(thumbnail);
            product.Thumbnail = newThumbnailFileName;
        }
        else
        {
            // No new thumbnail uploaded, retrieve the existing thumbnail filename from the database
            var existingProduct = _PitstopContext.Product.FirstOrDefault(p => p.Id == product.Id);
            if (existingProduct != null)
            {
                product.Thumbnail = existingProduct.Thumbnail;
            }
        }

        // Check if a new category is submitted
        if (product.Category == null)
        {
            // No new category submitted, retrieve the existing category from the database
            var existingProduct = _PitstopContext.Product.FirstOrDefault(p => p.Id == product.Id);
            if (existingProduct != null)
            {
                product.Category = existingProduct.Category;
            }
        }

        // Save changes to the database
        await _PitstopContext.SaveChangesAsync();

        // Return success response
        return Json(new { success = true, message = "Product updated successfully!" });
    }
    catch (DbUpdateConcurrencyException)
    {
        if (!ProductExists(product.Id))
        {
            return NotFound();
        }
        else
        {
            throw;
        }
    }
}


        private async Task<string> SaveThumbnailAsync(IFormFile thumbnail)
        {
            // Generate a unique file name for the thumbnail
            string fileName = Guid.NewGuid().ToString() + Path.GetExtension(thumbnail.FileName);
            
            // Define the path where the thumbnail will be saved
            string filePath = Path.Combine("wwwroot", "thumbnails", fileName);

            // Save the thumbnail file to the specified path
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await thumbnail.CopyToAsync(stream);
            }

            // Return the file name of the saved thumbnail
            return fileName;
        }

        private bool ProductExists(string id)
        {
            return _PitstopContext.Product.Any(e => e.Id == id);
        }
        
        public async Task<IActionResult> DeleteProduct(int id)
        {
            try
            {
                // Convert the id to string
                string productId = id.ToString();

                var productToDelete = await _PitstopContext.Product.FindAsync(productId);
                if (productToDelete == null)
                {
                    return NotFound();
                }

                _PitstopContext.Product.Remove(productToDelete);
                await _PitstopContext.SaveChangesAsync();
                
                // Optionally, you can return a success message or status
                return Ok(new { message = "Product deleted successfully." });
            }
            catch (Exception ex)
            {
                // Log the exception
                Console.WriteLine("Error deleting product: " + ex.Message);
                // Return an error message or status
                return StatusCode(500, new { error = "An error occurred while deleting the product." });
            }
        }

        public async Task<IActionResult> AddProduct(Product product, IFormFile Thumbnail, List<IFormFile> Media, string sizes)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    int nextId = await _PitstopContext.Product.CountAsync() + 1;
                    product.Id = nextId.ToString();

                    product.Size = sizes;

                    // Set the creation date to the current date and time
                    product.CreatedDate = DateTime.Now;

                    // Validate mandatory fields
                    if (string.IsNullOrEmpty(product.Name) || !IsValidQuantity(product.Qty) || !IsValidPrice(product.Price))
                    {
                        // Return validation error if mandatory fields are missing or invalid
                        return Json(new { success = false, message = "Validation failed", errors = new [] { "Name, Qty, and Price are mandatory fields." } });
                    }

                    // Handle Thumbnail file upload
                if (Thumbnail != null && Thumbnail.Length > 0)
                {
                    string uniqueFileName = nextId + "_" + Path.GetFileNameWithoutExtension(Thumbnail.FileName);
                    string thumbnailFolder = Path.Combine(_environment.WebRootPath, "Thumbnails");
                    string thumbnailPath = Path.Combine(thumbnailFolder, uniqueFileName + Path.GetExtension(Thumbnail.FileName)); // Include file extension here

                    if (!Directory.Exists(thumbnailFolder))
                    {
                        Directory.CreateDirectory(thumbnailFolder);
                    }

                    using (var stream = new FileStream(thumbnailPath, FileMode.Create))
                    {
                        await Thumbnail.CopyToAsync(stream);
                    }

                    product.Thumbnail = uniqueFileName + Path.GetExtension(Thumbnail.FileName); // Include file extension here
                }

                // Handle Media file upload (up to 3 files)
                foreach (var mediaFile in Media)
                {
                    if (mediaFile != null && mediaFile.Length > 0)
                    {
                        // Generate a unique file name for each media file
                        string uniqueFileName = nextId + "_" + Path.GetFileNameWithoutExtension(mediaFile.FileName);
                        string mediaFolder = Path.Combine(_environment.WebRootPath, "Media");
                        string mediaPath = Path.Combine(mediaFolder, uniqueFileName + Path.GetExtension(mediaFile.FileName)); // Include file extension here

                        if (!Directory.Exists(mediaFolder))
                        {
                            Directory.CreateDirectory(mediaFolder);
                        }

                        using (var stream = new FileStream(mediaPath, FileMode.Create))
                        {
                            await mediaFile.CopyToAsync(stream);
                        }

                        // Append the file name to the product's media property
                        product.Media += uniqueFileName + Path.GetExtension(mediaFile.FileName) + ";"; // Include file extension here
                    }
                }
                    _PitstopContext.Product.Add(product);
                    await _PitstopContext.SaveChangesAsync();

                    TempData["Message"] = "Product added successfully!";
                    return Json(new { success = true, message = "Product added successfully!" });
                }
                catch (Exception ex)
                {
                    TempData["ErrorMessage"] = "An error occurred while adding the product: " + ex.Message;
                    return Json(new { success = false, message = "An error occurred while adding the product: " + ex.Message });
                }
            }
            else
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                return Json(new { success = false, message = "Validation failed", errors = errors });
            }
        }

        private bool IsValidPrice(string price)
        {
            return !string.IsNullOrEmpty(price) && decimal.TryParse(price, out _);
        }

        private bool IsValidQuantity(string qty)
        {
            return !string.IsNullOrEmpty(qty) && int.TryParse(qty, out _);
        }


        private async Task<byte[]> GetMediaFileContentAsync(string mediaFileName)
        {
            try
            {
                // Construct the file path using the provided media file name
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Product", mediaFileName);

                // Read the file content as a byte array
                return await System.IO.File.ReadAllBytesAsync(filePath);
            }
            catch (Exception ex)
            {
                // Handle any errors that occur during file reading
                throw new Exception($"Failed to read media file: {ex.Message}");
            }
        }

#endregion

#region Category
        public IActionResult Categories()
        {
            return View();
        }
        public IActionResult GetDataCategory()
        {
            var categoryList = _PitstopContext.Category.ToList(); // Assuming your category model is named "Category"
            var categoryData = categoryList.Select(c => new
            {
                id = c.Id,
                name = c.Name,
                description = c.Description
            }).ToList();

            return Json(new { data = categoryData });
        }
       public IActionResult AddCategory(Category category)
        {
            try
            {
                int nextId = 1;
                if (_PitstopContext.Category.Any())
                {
                    // Fetch the IDs into memory and then find the max
                    nextId = _PitstopContext.Category.Select(c => int.Parse(c.Id)).AsEnumerable().Max() + 1;
                }
                category.Id = nextId.ToString();
                category.CreatedDate = DateTime.Now;

                _PitstopContext.Category.Add(category);
                _PitstopContext.SaveChanges();

                return Json(new { success = true, message = "Category added successfully!" });
            }
            catch (Exception ex)
            {
                // Log the inner exception
                if (ex.InnerException != null)
                {
                    Console.WriteLine("Inner Exception: " + ex.InnerException.Message);
                }

                // Handle exceptions and return error message
                return Json(new { success = false, message = "An error occurred while adding the category: " + ex.Message });
            }
        }

        public IActionResult GetCategoryById(string id)
        {
            var category = _PitstopContext.Category.Find(id);
            if (category != null)
            {
                return Json(new { success = true, data = category });
            }
            else
            {
                return Json(new { success = false, message = "Category not found." });
            }
        }
        public IActionResult EditCategory([FromBody]Category category)
        {
            try
            {
                // No need to convert, as Id is already expected to be a string
                var existingCategory = _PitstopContext.Category.Find(category.Id);
                if (existingCategory != null)
                {
                    existingCategory.Name = category.Name;
                    existingCategory.Description = category.Description;
                    // Update other properties as needed

                    _PitstopContext.Entry(existingCategory).State = EntityState.Modified;
                    _PitstopContext.SaveChanges();


                    return Json(new { success = true, message = "Category edited successfully!" });
                }
                else
                {
                    return Json(new { success = false, message = "Category not found." });
                }
            }
            catch (Exception ex)
            {
                // Log or handle the exception as necessary
                return Json(new { success = false, message = "An error occurred while editing the category: " + ex.Message });
            }
        }

        public IActionResult DeleteCategory(string id)
        {
            try
            {
                var category = _PitstopContext.Category.Find(id);
                
                if (category == null)
                {
                    return Json(new { success = false, message = "Category not found." });
                }
                
                _PitstopContext.Category.Remove(category);
                _PitstopContext.SaveChanges();
                
                return Json(new { success = true, message = "Category deleted successfully." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "An error occurred while deleting the category: " + ex.Message });
            }
        }

#endregion

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
            var products = _PitstopContext.Product.Select(p => new
            {
                Value = p.Id, // Assuming p.Id is already a string
                Text = p.Name
            }).ToList();

            ViewBag.Products = new SelectList(products, "Value", "Text");

            // Initialize ViewBag.SelectedFeaturedItems here
            ViewBag.SelectedFeaturedItems = new Dictionary<string, string>(); // Or initialize it with appropriate data

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

        public IActionResult Carousel2Upload(IFormFile imageFile)
        {
            try
            {
                if (imageFile != null && imageFile.Length > 0)
                {
                    // Generate unique file name
                    string uniqueFileName = $"{Guid.NewGuid().ToString()}_{imageFile.FileName}";

                    // Save the file to the server
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

                    // Create a new Section2Media entry
                    var newMediaEntry = new Section2Media
                    {
                        FileName = uniqueFileName,
                        FileType = Path.GetExtension(imageFile.FileName),
                        UploadDate = DateTime.Now,
                        Caption = "Optional caption",
                    };

                    _PitstopContext.Section2Media.Add(newMediaEntry);
                    _PitstopContext.SaveChanges();

                    return Ok(new { success = true, message = "Image uploaded successfully" });
                }
                else
                {
                    return BadRequest("No file selected for upload.");
                }
            }
            catch (Exception ex)
            {
                // Log the exception including inner exception details
                _logger.LogError(ex, "An error occurred while saving entity changes");

                // Return an error response
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        public IActionResult UploadFeaturedItem(IFormFile imageFile, string title, string description, string price, int order)
        {
            try
            {
                if (imageFile != null && imageFile.Length > 0)
                {
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
                        Description = description,
                        Price = price,
                        Order = order.ToString(), // Convert order to string
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
        public async Task<IActionResult> SaveFeaturedSelection(string order, string id)
        {
            try
            {
                if (string.IsNullOrEmpty(id) || string.IsNullOrEmpty(order))
                {
                    return Json(new { success = false, message = "Order and Product ID are required." });
                }

                var product = await _PitstopContext.Product.FirstOrDefaultAsync(p => p.Id == id);
                if (product == null)
                {
                    return Json(new { success = false, message = "Product not found." });
                }

                var existingFeaturedItem = await _PitstopContext.FeaturedItems.FirstOrDefaultAsync(fi => fi.Order == order);

                if (existingFeaturedItem != null)
                {
                    // Update existing featured item with the new product details
                    UpdateFeaturedItemFromProduct(existingFeaturedItem, product);
                    _PitstopContext.Entry(existingFeaturedItem).State = EntityState.Modified;
                }
                else
                {
                    // If no existing featured item found, create a new one
                    var newFeaturedItem = new FeaturedItem
                    {
                        Id = Guid.NewGuid().ToString(),
                        Order = order,
                        ProductId = product.Id,
                        FileName = product.Thumbnail,
                        Title = product.Name,
                        Price = product.Price,
                        Description = product.Description,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    };
                    _PitstopContext.FeaturedItems.Add(newFeaturedItem);
                }

                await _PitstopContext.SaveChangesAsync();
                return Json(new { success = true, message = "Featured item updated successfully!" });
            }
            catch (Exception ex)
            {
                // Log the exception for troubleshooting
                Console.WriteLine($"Error saving featured selection: {ex}");
                return Json(new { success = false, message = "An error occurred while updating the featured item." });
            }
        }   

        private void UpdateFeaturedItemFromProduct(FeaturedItem featuredItem, Product product)
        {
            // Update properties of the existing featured item with values from the product
            featuredItem.FileName = product.Thumbnail;
            featuredItem.Title = product.Name;
            featuredItem.Price = product.Price;
            featuredItem.Description = product.Description;
            featuredItem.UpdatedAt = DateTime.UtcNow;
        }

        public IActionResult FeaturedItemSettings()
        {
            var products = _PitstopContext.Product.Select(p => new
            {
                Id = p.Id.ToString(),
                Name = p.Name
            }).ToList();

            var selectedFeaturedItems = _PitstopContext.FeaturedItems.ToDictionary(fi => fi.Order, fi => fi.Id);

            ViewBag.Products = new SelectList(products, "Value", "Text");
            ViewBag.SelectedFeaturedItems = selectedFeaturedItems;

            return View();
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
