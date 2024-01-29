using Pitstop.Models;
using Pitstop.Models.PitstopData;
using Pitstop.Service;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Security.Claims;
using Microsoft.Extensions.Options;
using Pitstop.Models.Common;
using Pitstop.Helper;


namespace Pitstop.Controllers
{
    public class AuthController : Controller
    {
        private readonly UtilityClass _utility;
        private readonly AppKeysOption _options;
        private readonly PitstopContext _PitstopContext;
        private readonly ILogger<AuthController> _logger;
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<Role> _roleManager;
        private readonly SignInManager<User> _signInManager;
        private readonly CommonService _commonService;
        private readonly EmailService _emailService;

        public AuthController(
          ILoggerFactory loggerFactory,
          PitstopContext PitstopContext, UserManager<User> userManager, RoleManager<Role> roleManager, SignInManager<User> signInManager,
          CommonService commonService, UtilityClass utility, IOptions<AppKeysOption> options,EmailService emailService)
        {
            _logger = loggerFactory.CreateLogger<AuthController>();
            _PitstopContext = PitstopContext;
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;
            _commonService = commonService;
            _emailService = emailService;
            _utility = utility;
            _options = options.Value;
        }

        public IActionResult Index()
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }
            var model = new UserLoginModel();
            model.DateNow = DateTime.Now;
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Login(UserLoginModel param)
        {
            try
            {
                var login = await _signInManager.PasswordSignInAsync(param.Username,
                    param.Password, param.RememberMe, lockoutOnFailure: true);

                if (login.Succeeded)
                {
                    return Ok(new ResponseViewModel<object>
                    {
                        Message = "Login Success"
                    });
                }

                if (login.IsLockedOut)
                {
                    return StatusCode(StatusCodes.Status403Forbidden, "Account is Locket out.");
                }
                else
                {
                    return StatusCode(StatusCodes.Status401Unauthorized, "Failed login attempt.");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        public async Task<IActionResult> Logout()
        {
            try
            {
                await HttpContext.SignOutAsync("Identity.Application");
                return RedirectToAction(nameof(AuthController.Index), "Auth");
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        public IActionResult ForgotPasswordPage()
        {
            return View();
        }

        public IActionResult ForgotPassword(string loginId)
        {
            if (string.IsNullOrEmpty(loginId))
            {
                return BadRequest(new ResponseViewModel<object>
                {
                    Message = "Cannot find the user."
                });
            }

            var user = _PitstopContext.Users.FirstOrDefault(e => e.NormalizedEmail == loginId.ToUpper());

            if (user == null)
            {
                return BadRequest(new ResponseViewModel<object>
                {
                    Message = "Cannot find the user."
                });
            }

            var emailSubmittedId = Guid.NewGuid().ToString();

            //Send email for request password.
            var sendRequestPassword = _emailService.SendEmailRequestResetPassword(user.Id, emailSubmittedId);
            if (sendRequestPassword != "Success")
            {
                return BadRequest(new ResponseViewModel<object>
                {
                    Message = sendRequestPassword
                });
            }

            return Ok(new ResponseViewModel<object>
            {
                Message = "Your request to reset your password has been sent to your email. Please check your email inbox."
            });
        }

        public async Task<IActionResult> ResetMyPasswordAsync(string? id)
        {
            ResetMyPasswordModel model = new ResetMyPasswordModel();

            using (var transaction = _PitstopContext.Database.BeginTransaction())
            {
                try
                {
                    if (id == null)
                    {
                        model = new ResetMyPasswordModel
                        {
                            Message = "Cannot find any action.",
                            UserId = id,
                            Icon = "error"
                        };
                        return View(model);
                    }

                    var emailSubmitted = _PitstopContext.EmailSubmitteds.Find(id);

                    if (emailSubmitted == null)
                    {
                        model = new ResetMyPasswordModel
                        {
                            Message = "Cannot find any action.",
                            UserId = id,
                            Icon = "error"
                        };
                        return View(model);
                    }

                    if (emailSubmitted.IsValid == false)
                    {
                        model = new ResetMyPasswordModel
                        {
                            Message = "You already done this request, please try again later.",
                            UserId = id,
                            Icon = "error"
                        };
                        return View(model);
                    }

                    var newPassword = _commonService.GetCharacterRandomString(12);
                    var user = _PitstopContext.Users.Find(emailSubmitted.UserId);
                    string code = await _userManager.GeneratePasswordResetTokenAsync(user);
                    var result = await _userManager.ResetPasswordAsync(user, code, newPassword);

                    if (result.Succeeded)
                    {
                        emailSubmitted.IsValid = false;
                        _PitstopContext.EmailSubmitteds.Update(emailSubmitted);
                        _PitstopContext.SaveChanges();

                        var emailSubmittedId = Guid.NewGuid().ToString();
                        var sendNewPassword = _emailService.SendNewGeneratedPassword(user.Id, newPassword, emailSubmittedId, AppConstant.DataControl.EmailSubmitTypeData.RequestResetPassword);

                        if (sendNewPassword != "Success")
                        {
                            model = new ResetMyPasswordModel
                            {
                                Message = "Failed to send or resetting your password, please contact administrator.",
                                UserId = id,
                                Icon = "error"
                            };
                            return View(model);
                        }

                        transaction.Commit();

                        model = new ResetMyPasswordModel
                        {
                            Message = "Resetting your password is success. Please check your email for your new password!",
                            UserId = id,
                            Icon = "success"
                        };
                        return View(model);
                    }

                    model = new ResetMyPasswordModel
                    {
                        Message = "Failed to send or resetting your password, please contact administrator.",
                        UserId = id,
                        Icon = "error"
                    };
                    return View(model);
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    model = new ResetMyPasswordModel
                    {
                        Message = "Failed to send or resetting your password, please contact administrator.",
                        UserId = id,
                        Icon = "error"
                    };
                    return View(model);
                }
            }
        }
        [HttpPost]
        public async Task<IActionResult> Settings(UserSettingsModel param, string submitType)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId == null)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Cannot find the user.");
            }

            try
            {
                if (submitType == AppConstant.SubmitTypeUserSettings.ChangePassword)
                {
                    if (param.ChangePassword.NewPassword != param.ChangePassword.ConfirmNewPassword)
                    {
                        return BadRequest(new ResponseViewModel<object>
                        {
                            Message = "Your new password and confirm new password is incorrect! Please check again."
                        });
                    }

                    var user = _PitstopContext.Users.Find(userId);

                    if (user != null)
                    {
                        var result = await _userManager.ChangePasswordAsync(user, param.ChangePassword.Current, param.ChangePassword.NewPassword);
                        if (result.Succeeded)
                        {
                            await _signInManager.SignInAsync(user, isPersistent: false);
                            return Ok(new ResponseViewModel<object>
                            {
                                Message = "Update new password success"
                            });
                        }

                        string errorMsg = string.Empty;

                        foreach (var item in result.Errors)
                        {
                            errorMsg += $"{item.Description} \n";
                        }

                        return BadRequest(new ResponseViewModel<object>
                        {
                            Message = errorMsg
                        });
                    }
                }
                else if (submitType == AppConstant.SubmitTypeUserSettings.ChangeMobileNumber)
                {
                    var user = _PitstopContext.Users.Find(userId);

                    if (user != null)
                    {
                        user.PhoneNumber = param.ChangeMobileNumber.MobileNumber;

                        _PitstopContext.Users.Update(user);

                        bool isSuccess = await _PitstopContext.SaveChangesAsync() > 0;

                        if (isSuccess)
                        {
                            return Ok(new ResponseViewModel<object>
                            {
                                Message = "Success update mobile number"
                            });
                        }
                        else
                        {
                            return BadRequest(new ResponseViewModel<object>
                            {
                                Message = "Failed to update mobile number"
                            });
                        }
                    }
                }

                return BadRequest(new ResponseViewModel<object>
                {
                    Message = "Something went wrong"
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseViewModel<object>
                {
                    Message = ex.Message
                });
            }
        }

        public IActionResult GetUserInformation()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var user = _PitstopContext.Users.Find(userId);

            var model = new UserInformationModel
            {
                Name = user.UserName,
                Email = user.Email
            };

            return Json(Ok(model));
        }
        [Route("AccessDenied")]
        public IActionResult AccessDenied()
        {
            return View();
        }
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

    }
}