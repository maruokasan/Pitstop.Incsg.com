using Pitstop.Helper;
using Pitstop.Models;
using Pitstop.Models.PitstopData;
using Pitstop.Models.DataTable;
using Pitstop.Service;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Reflection;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.Extensions.Options;
using Pitstop.Models.Common;

namespace Pitstop.Controllers
{
    
    public class UserMasterController : Controller
    {
        private readonly UtilityClass _utility;
        private readonly AppKeysOption _options;
        private readonly PitstopContext _PitstopContext;
        private readonly ILogger<UserMasterController> _logger;
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<Role> _roleManager;
        private readonly SignInManager<User> _signInManager;
        private readonly CommonService _commonService;
        private readonly EmailService _emailService;
        public UserMasterController(
          ILoggerFactory loggerFactory,
          PitstopContext PitstopContext, UserManager<User> userManager, RoleManager<Role> roleManager, SignInManager<User> signInManager, CommonService commonService,        UtilityClass utility,
          IOptions<AppKeysOption> options, EmailService emailService)
        {
            _logger = loggerFactory.CreateLogger<UserMasterController>();
            _PitstopContext = PitstopContext;
            _userManager = userManager;
            _roleManager = roleManager;
            _commonService = commonService;
            _signInManager = signInManager;
            _emailService = emailService;
            _utility = utility;
            _options = options.Value;
        }

        // [Authorize(ClaimType = AppConstant.RoleClaim.ClaimType.SSP, ClaimValue = AppConstant.RoleClaim.ClaimValue.CreateEditAccounts)]
        public IActionResult Index()
        {
            ViewBag.Applications = _PitstopContext.Systems.ToList();
            ViewBag.StatusList = _commonService.GetStatusList();
            ViewBag.RoleList = _commonService.GetRoleList();

            return View();
        }

        public IActionResult UserView()
        {
            return View();
        }

        public IActionResult Permissions()
        {
            return View();
        }

        public IActionResult Rolelist()
        {
            return View();
        }

        public IActionResult Roleview()
        {
            return View();
        }
        // [Authorize(ClaimType = AppConstant.RoleClaim.ClaimType.SSP, ClaimValue = AppConstant.RoleClaim.ClaimValue.SetDeactiveAccountSettings)]
        public IActionResult AccountSettings()
        {
            AccountSettingsModel model = new AccountSettingsModel();

            var parentData = _commonService.GetDataControlData(null, AppConstant.DataControl.DeactiveAccountInactiveForNumberOfDays, null);

            if (parentData == null)
            {
                return View(model);
            }

            var result = _PitstopContext.DataControls.FirstOrDefault(e => e.Parent == parentData.Id);

            if (result == null)
            {
                return View(model);
            }

            model.NumberOfDaysDeactiveAccount = result.Description;

            return View(model);
        }

        // [Authorize(ClaimType = AppConstant.RoleClaim.ClaimType.SSP, ClaimValue = AppConstant.RoleClaim.ClaimValue.SetRoles)]
        public IActionResult Role()
        {
            return View();
        }
       public async Task<IActionResult> CreateUpdateUser(CreateUpdateUserMasterDataModel param, string SubmitType, string accessTypeValues)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var accessType = accessTypeValues != null ? accessTypeValues.Split(",") : null;

                if (SubmitType == AppConstant.SubmitCreateUpdateUsers.Save && string.IsNullOrEmpty(param.UserId))
                {
                    var user = new User
                    {
                        Id = Guid.NewGuid().ToString(),
                        EmployeeId = 0,
                        UserName = param.UserName,
                        NormalizedUserName = param.UserName.ToUpper(),
                        FirstName = param.FullName,
                        Email = param.EmailId,
                        NormalizedEmail = param.EmailId.ToUpper(),
                        EmailConfirmed = false,
                        PhoneNumber = param.MobileNumber,
                        PhoneNumberConfirmed = false,
                        CreatedBy = userId,
                        CreatedDate = DateTime.Now,
                        IsActive = true,
                        IsUserActive = true,
                        LastAccessDate = DateTime.Now,
                    };

                    // Fetch accesstypeid based on the selected accessTypeValues
                    string roleid = FetchAccessTypeID(accessTypeValues);
                    user.AccessTypeId = roleid;

                    var isUserExist = _PitstopContext.Users.FirstOrDefault(e => e.NormalizedEmail == user.NormalizedEmail);

                    var isUserNameExist = _PitstopContext.Users.FirstOrDefault(e => e.NormalizedUserName == user.NormalizedUserName);

                    if (isUserExist != null)
                    {
                        return BadRequest(new ResponseViewModel<object>
                        {
                            Message = "This email already exist."
                        });
                    }

                    if (isUserNameExist != null)
                    {
                        return BadRequest(new ResponseViewModel<object>
                        {
                            Message = "This username already exist."
                        });
                    }

                    using (var transaction = _PitstopContext.Database.BeginTransaction())
                    {
                        var password = _commonService.GetCharacterRandomString(12);

                        var result = await _userManager.CreateAsync(user, password);

                        if (!result.Succeeded)
                        {
                            transaction.Rollback();
                            return BadRequest(new ResponseViewModel<object>
                            {
                                Message = "Failed for add new user."
                            });
                        }
                        if (accessType != null)
                        {
                            foreach (var accessTypeId in accessType)
                            {
                                var userRole = new UserRoles
                                {
                                    RoleId = accessTypeId,
                                    UserId = user.Id
                                };

                                _PitstopContext.UserRoles.Add(userRole);
                            }
                        }

                        if (!string.IsNullOrEmpty(param.ApplicationNames))
                        {
                            try
                            {
                                var listApps = param.ApplicationNames.Split(",");

                                for (int i = 0; i < listApps.Count(); i++)
                                {
                                    var systemMapping = new UserSystemMapping
                                    {
                                        Id = Guid.NewGuid().ToString(),
                                        UserId = user.Id,
                                        SystemId = _commonService.GetIdOfSystemByName(listApps[i])
                                    };

                                    _PitstopContext.UserSystemMappings.Add(systemMapping);
                                }
                            }
                            catch
                            {
                                return BadRequest(new ResponseViewModel<object>
                                {
                                    Message = "Something went wrong while insert the system apps."
                                });
                            }
                        }      

                        // var msgUserMappingJobcode = InsertUpdateUserMappingJobcode(user.Id, jobcodeType);  
                        _PitstopContext.SaveChanges();          

                        var emailSubmittedId = Guid.NewGuid().ToString();
                        var sendPasswordToEmail = _emailService.SendNewGeneratedPassword(user.Id, password, emailSubmittedId, AppConstant.DataControl.EmailSubmitTypeData.CreateUserPassword);

                        if (sendPasswordToEmail != "Success")
                        {
                            //transaction.Rollback();
                            transaction.Commit();
                            return BadRequest(new ResponseViewModel<object>
                            {
                                Message = "Account created successfully but sending email failed."
                            });
                        }
                        transaction.Commit();
                    }

                    return Ok(new ResponseViewModel<object>
                    {
                        Message = "Success create the user. Please check user email for the password."
                    });
                }
                else
                {
                    if (param.UserId != null)
                    {
                        //var isUserExist = _haileckPortalContext.Users.FirstOrDefault(e => e.NormalizedEmail == param.EmailId.ToUpper()
                        //&& e.NormalizedUserName == param.FullName.ToUpper());

                        //if (isUserExist != null)
                        //{
                        //    return BadRequest(new ResponseViewModel<object>
                        //    {
                        //        Message = "This user already exist."
                        //    });
                        //}

                        var user = _PitstopContext.Users.Find(param.UserId);

                        if (user != null)
                        {
                            user.UserName = param.UserName;
                            user.FirstName = param.FullName;
                            user.NormalizedUserName = param.UserName.ToUpper();
                            user.Email = param.EmailId;
                            user.NormalizedEmail = param.EmailId.ToUpper();
                            user.PhoneNumber = param.MobileNumber;
                            user.UpdatedBy = userId;
                            user.UpdatedDate = DateTime.Now;
                            user.IsUserActive = param.IsActive;

                    // Remove existing user roles and user jobs
                    _PitstopContext.UserRoles.RemoveRange(_PitstopContext.UserRoles.Where(e => e.UserId == user.Id));
                            
                             if (accessType != null)
                            {
                                foreach (var accessTypeId in accessType)
                                {
                                    var userRole = new UserRoles
                                    {
                                        RoleId = accessTypeId,
                                        UserId = user.Id
                                    };

                                    _PitstopContext.UserRoles.Add(userRole);
                                }
                            }

                    // Update AccessTypeId based on accesstypeValues
                    string roleid = FetchAccessTypeID(accessTypeValues);
                    user.AccessTypeId = roleid;

                    _PitstopContext.Users.Update(user);

                            var listUserSystem = _PitstopContext.UserSystemMappings.Where(e => e.UserId == user.Id).ToList();
                            _PitstopContext.UserSystemMappings.RemoveRange(listUserSystem);

                            if (!string.IsNullOrEmpty(param.ApplicationNames))
                            {
                                try
                                {
                                    var listApps = param.ApplicationNames.Split(",");
                                    for (int i = 0; i < listApps.Count(); i++)
                                    {
                                        var systemMapping = new UserSystemMapping
                                        {
                                            Id = Guid.NewGuid().ToString(),
                                            UserId = user.Id,
                                            SystemId = _commonService.GetIdOfSystemByName(listApps[i])
                                        };

                                        _PitstopContext.UserSystemMappings.Add(systemMapping);
                                    }
                                }
                                catch
                                {
                                    return BadRequest(new ResponseViewModel<object>
                                    {
                                        Message = "Something went wrong while insert the system apps."
                                    });
                                }
                            }
                            var isSuccess = await _PitstopContext.SaveChangesAsync() > 0;

                            if (isSuccess)
                            {
                                return Ok(new ResponseViewModel<object>
                                {
                                    Message = "Success update the user."
                                });
                            }
                            else
                            {
                                return BadRequest(new ResponseViewModel<object>
                                {
                                    Message = "Failed while update the user data."
                                });
                            }
                        }
                        else
                        {
                            return BadRequest(new ResponseViewModel<object>
                            {
                                Message = "Cannot find the user."
                            });
                        }
                    }
                    else
                    {
                        return BadRequest(new ResponseViewModel<object>
                        {
                            Message = "Cannot find the user."
                        });
                    }
                }

            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseViewModel<object>
                {
                    Message = ex.Message
                });
            }
        }
        private string FetchAccessTypeID(string accessTypeValues)
        {
            if (accessTypeValues == null || accessTypeValues.Length == 0)
            {
                // Handle the case when no access type values are selected
                return null;
            }

            // Split the selected accessTypeValues
            var selectedRoles = accessTypeValues.Split(",");

            // Fetch the roleids based on selectedRoles from the Roles table
            var roleIds = _PitstopContext.Roles
                .Where(role => selectedRoles.Contains(role.Id))
                .Select(role => role.Id)
                .ToList();

            // Join the roleids into a comma-separated string
            var result = string.Join(",", roleIds);

            return result;
        }
      
       public IActionResult GetDataUserMaster(KTParameters parameters)
        {
            string generalSearch = HttpContext.Request.Form["query[generalSearch]"].ToString() != null ? HttpContext.Request.Form["query[generalSearch]"].ToString() : "";
            string statusSearch = HttpContext.Request.Form["query[Status]"].ToString() != null ? HttpContext.Request.Form["query[Status]"].ToString() : "";
            string roleSearch = HttpContext.Request.Form["query[Role]"].ToString() != null ? HttpContext.Request.Form["query[Role]"].ToString() : "";

            string? role = !string.IsNullOrEmpty(roleSearch) ? roleSearch : null;
            bool? statusBool = !string.IsNullOrEmpty(statusSearch) ? statusSearch == "1" ? true : false : null;

            var totalData = _PitstopContext.Users.Where(e => (e.NormalizedUserName.Contains(generalSearch)
            || e.NormalizedEmail.Contains(generalSearch)) && e.IsActive == true
            && (statusBool != null ? e.IsUserActive == statusBool : e.IsUserActive != null)
            && (role != null ? e.AccessTypeId == role : e.AccessTypeId != null)
            ).Count();

            var skip = (parameters.pagination.page - 1) * parameters.pagination.perpage;
            var query = _PitstopContext.Users.AsEnumerable();

            if (parameters.sort != null)
            {
                PropertyInfo pi = typeof(User).GetProperty(parameters.sort.field);

                if (pi == null)
                {
                    if (parameters.sort.field.Split(".").Count() > 1)
                    {
                        //child sorting
                    }
                }
                else
                {
                    if (parameters.sort.sort == "desc")
                    {
                        query = query.OrderByDescending(x => pi.GetValue(x, null));
                    }
                    else
                    {
                        query = query.OrderBy(x => pi.GetValue(x, null));
                    }
                }
            }

            var userMasterCollection = query.Where(e => (e.NormalizedUserName.Contains(generalSearch.ToUpper())
            || e.NormalizedEmail.Contains(generalSearch.ToUpper())) && e.IsActive == true
            && (statusBool != null ? e.IsUserActive == statusBool : e.IsUserActive != null)
            && (role != null ? e.AccessTypeId == role : e.AccessTypeId != null))
                .Skip(skip).Take(parameters.pagination.perpage).ToList();

            var result = new List<UserMasterDataList>(userMasterCollection.Select(e => new UserMasterDataList
            {
                UserId = e.Id,
                Name = e.NormalizedUserName,
                StaffId = e.EmployeeId.ToString(),
                Email = e.Email,
                Status = e.IsUserActive == true ? "Active" : "Inactive",
                Access = _commonService.GetAccessName(e.Id),
                Role = _commonService.GetDescriptionOfRole(e.Id),
                CreatedBy = _commonService.GetUserName(e.CreatedBy),
                CreatedDate = e.CreatedDate.ToShortDateString()
            }));

            parameters.pagination.total = result != null && result.Count != 0 ? Convert.ToInt32(totalData) : 0;

            return Json(new KTResult<List<UserMasterDataList>>
            {
                data = result,
                meta = parameters.pagination
            });
        }

        public IActionResult GetUserData(string userId)
        {
            try
            {
                if (userId != null)
                {
                    var user = _PitstopContext.Users.Find(userId);

                    if (user == null)
                    { 
                        return BadRequest(new ResponseViewModel<object>
                        {
                            Message = "Cannot find the user."
                        });
                    }

                    var userSystemApps = _PitstopContext.UserSystemMappings.Where(e => e.UserId == userId).ToList();

                    var userRoleList = _PitstopContext.UserRoles.Where(e => e.UserId == userId).ToList();
                    var listAccessTypeMapping = new List<RoleModel>();

                    foreach (var item in userRoleList)
                    {
                        var role = _PitstopContext.Roles.Find(item.RoleId);
                        listAccessTypeMapping.Add(new RoleModel
                        {
                            Id = role.Id,
                            Name = role.Name
                        });
                    }

                    CreateUpdateUserMasterDataModel result = new CreateUpdateUserMasterDataModel
                    {
                        UserName = user.UserName,
                        UserId = user.Id,
                        EmailId = user.Email,
                        FullName = user.FirstName,
                        MobileNumber = user.PhoneNumber,
                        IsActive = user.IsUserActive.GetValueOrDefault(false),
                        // AccessTypeId = user.AccessTypeId,
                        // AccessTypeName = _commonService.GetDescriptionOfRole(user.AccessTypeId),
                        UserSystemApps = userSystemApps.Count != 0 ? userSystemApps.Select(e => new string(_commonService.GetNameOfSystemById(e.SystemId))).ToList() : null,
                        AccessTypeList = listAccessTypeMapping.Count != 0 ? listAccessTypeMapping : new List<RoleModel>()
                    };

                    return Ok(new ResponseViewModel<CreateUpdateUserMasterDataModel>
                    {
                        Data = result,
                        Message = "Find it."
                    });
                }
                else
                {
                    return BadRequest(new ResponseViewModel<object>
                    {
                        Message = "Cannot find the user."
                    });
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseViewModel<object>
                {
                    Message = ex.Message
                });
            }
        }


        public IActionResult GetDataRoleMaster(KTParameters parameters)
        {
            string generalSearch = HttpContext.Request.Form["query[generalSearch]"].ToString() != null ? HttpContext.Request.Form["query[generalSearch]"].ToString() : "";

            var totalData = _PitstopContext.Roles.Where(e => e.NormalizedName.Contains(generalSearch)).Count();

            var skip = (parameters.pagination.page - 1) * parameters.pagination.perpage;

            var roleCollection = _PitstopContext.Roles.Where(e => e.NormalizedName.Contains(generalSearch))
                .Skip(skip).Take(parameters.pagination.perpage).ToList();


            var result = new List<RoleMasterDataList>();

            foreach (var item in roleCollection)
            {
                var data = new RoleMasterDataList
                {
                    RoleId = item.Id,
                    Name = item.Name,
                    Action = "TEST"
                };
                result.Add(data);
            }

            parameters.pagination.total = result != null && result.Count != 0 ? Convert.ToInt32(totalData) : 0;

            return Json(new KTResult<List<RoleMasterDataList>>
            {
                data = result,
                meta = parameters.pagination
            });
        }

        public async Task<IActionResult> ResetUserPassword(string userId)
        {
            if (userId == null)
            {
                return BadRequest(new ResponseViewModel<object>
                {
                    Message = "User not found!"
                });
            }

            using (var transaction = _PitstopContext.Database.BeginTransaction())
            {
                try
                {
                    var newPassword = _commonService.GetCharacterRandomString(12);
                    var user = _PitstopContext.Users.Find(userId);
                    string code = await _userManager.GeneratePasswordResetTokenAsync(user);
                    var result = await _userManager.ResetPasswordAsync(user, code, newPassword);

                    if (result.Succeeded)
                    {
                        var emailSubmittedId = Guid.NewGuid().ToString();
                        var sendNewPassword = _emailService.SendNewGeneratedPassword(user.Id, newPassword, emailSubmittedId, AppConstant.DataControl.EmailSubmitTypeData.AdminResetPassword);

                        if (sendNewPassword != "Success")
                        {
                            return BadRequest(new ResponseViewModel<object>
                            {
                                Message = $"Failed to send or resetting {user.UserName} password, please contact administrator."
                            });
                        }

                        transaction.Commit();

                        return Ok(new ResponseViewModel<object>
                        {
                            Message = $"Resetting {user.UserName} password is success. Please check {user.Email} email inbox for the new password!"
                        });
                    }

                    return BadRequest(new ResponseViewModel<object>
                    {
                        Message = $"Failed to send or resetting {user.UserName} password, please contact administrator."
                    });
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    return BadRequest(new ResponseViewModel<object>
                    {
                        Message = $"Something went wrong while do this process, please contact administrator."
                    });
                }
            }
        }
        [HttpPost]
        public IActionResult DeleteUser(string userId)
        {
            try
            {
                var user = _PitstopContext.Users.Find(userId);
                if (user != null)
                {
                    // Note: You may want to perform additional validation or checks before deleting the user.
                    _PitstopContext.Users.Remove(user);
                    _PitstopContext.SaveChanges();

                    return Ok(new { success = true });
                }
                else
                {
                    return NotFound(new { success = false, message = "User not found." });
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        [HttpGet]   
        [Route("UserMaster/RoleAssign/{id}")]
        public IActionResult RoleAssign(string id)
        {
            var systemPermissions = _PitstopContext.SystemPermissions.ToList();
            var systems = _PitstopContext.Systems.ToList();
            var roleClaims = _PitstopContext.RolePermissions.Where(e => e.RoleId == id).ToList();
            var systemPermissionsUnAssigned_Query = systemPermissions.Join(
                                              systems, sp => sp.SystemId, s => s.Id,
                                              (sp, s) => new { sp, s }).Select(r => new RolePermission { Id = r.sp.Id, ClaimValue = r.sp.Name, ClaimType = r.s.Name }).ToList();
            var systemPermissionsAssigned_Query = systemPermissionsUnAssigned_Query.Join
                                            (roleClaims,
                                            sps => new { sps.ClaimType, sps.ClaimValue },
                                            rc => new { rc.ClaimType, rc.ClaimValue },
                                            (sps, rc) => sps);

            List<RolePermission> systemPermissionsUnAssigned = systemPermissionsUnAssigned_Query.ToList();
            List<RolePermission> systemPermissionsAssigned = systemPermissionsAssigned_Query.ToList();

            ViewBag.SystemPermissions = systemPermissions;
            ViewBag.Systems = systems;
            ViewBag.RoleClaims = roleClaims;
            ViewBag.SystemPermissionsUnAssigned = systemPermissionsUnAssigned;
            ViewBag.SystemPermissionsAssigned = JsonConvert.SerializeObject(systemPermissionsAssigned);

            RoleModel data = new RoleModel();
            if (!string.IsNullOrEmpty(id))
            {
                var result = _PitstopContext.Roles.FirstOrDefault(x => x.Id == id);

                if (result != null)
                {
                    data.Name = result.Name;
                    return View(data);
                }
            }
            return View(new RoleModel());
        }

        [HttpPost]
        public IActionResult AssignRole(RoleModel param)
        {
            try
            {
                var systems = _PitstopContext.Systems.ToList();
                var systemPermissions = _PitstopContext.SystemPermissions.ToList();
                var oldData = _PitstopContext.RolePermissions.Where(d => d.RoleId == param.Id);
                _PitstopContext.RemoveRange(oldData);
                _PitstopContext.SaveChanges();

                if (param.SystemPermissionIds is not null)
                {
                    var systemPermissionIds = param.SystemPermissionIds.Split(",");
                    foreach (var systemPermissionId in systemPermissionIds)
                    {
                        RolePermission data = new RolePermission();
                        var result = systemPermissions.FirstOrDefault(d => d.Id == Convert.ToInt32(systemPermissionId));
                        data.RoleId = param.Id;
                        data.ClaimValue = result.Name;
                        data.ClaimType = systems.FirstOrDefault(d => d.Id == result.SystemId).Name;
                        _PitstopContext.RolePermissions.Add(data);
                        _PitstopContext.SaveChanges();
                    }
                }

                return Ok(new ResponseViewModel<object>
                {
                    Message = "Success saving the data."
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseViewModel<object>
                {
                    Message = "Success saving the data."
                });
            }


        }

        public IActionResult AccessTypeDataList(string searchTerm, string type)
        {
            var data = _PitstopContext.Roles.Where(e => e.Name != null).ToList();

            var setDataToModel = data.Select(e => new
            {
                id = e.Id,
                text = $"{e.Name}"
            }).ToList();

            searchTerm = searchTerm == null ? "" : searchTerm.ToUpper();
            var filteredData = setDataToModel.Where(e => e.text.ToUpper().Contains(searchTerm));

            return Json(filteredData);
        }

        public IActionResult Settings()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var user = _PitstopContext.Users.Find(userId);

            UserSettingsModel data = new UserSettingsModel();
            if (user != null)
            {
                data.ChangeMobileNumber.MobileNumber = user.PhoneNumber;
            }

            return View(data);
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
        //End UserMaster//

    }
}


