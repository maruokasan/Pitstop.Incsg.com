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
            var latestCarousel2 = _commonService.GetLatestCarousel2();

            var viewModel = new HomepageModel
            {
                LatestCarousel2 = latestCarousel2,
            };

            if (User.Identity.IsAuthenticated)
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var user = await _userManager.FindByIdAsync(userId);
                var roles = await _userManager.GetRolesAsync(user);

                viewModel.UserName = user?.UserName; // Assuming the User entity has a Name property
                viewModel.UserEmail = user?.Email;
                viewModel.UserRole = roles.FirstOrDefault();
            }
            return View(viewModel);
        }

        public IActionResult Cart()
        {
            return View();
        }

    }
}
