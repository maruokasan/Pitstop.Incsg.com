using Pitstop.Helper;
using Pitstop.Models;
using Pitstop.Models.DataTable;
using Pitstop.Models.PitstopData;
using Pitstop.Service;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Reflection;
using System.Security.Claims;
using Microsoft.Extensions.Options;
using Pitstop.Models.Common;

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

        // [Authorize(ClaimType = AppConstant.RoleClaim.ClaimType.SSP, ClaimValue = AppConstant.RoleClaim.ClaimValue.Dashboard_D)]
        public IActionResult Index()
        {
            return View();
        }
    }
}
