using Pitstop.Models;
using Pitstop.Models.PitstopData;
using Pitstop.Service;
using System.Globalization;

namespace Pitstop.Helper
{
    public class BackgroundWorkerService : BackgroundService
    {
        private readonly ILogger<BackgroundWorkerService> _logger;
        private readonly IServiceProvider _serviceProvider;
        public BackgroundWorkerService(ILogger<BackgroundWorkerService> logger, IServiceProvider serviceProvider)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                using IServiceScope scope = _serviceProvider.CreateScope();

                await using PitstopContext _haileckApplicationContext =
                    scope.ServiceProvider.GetRequiredService<PitstopContext>();

                CommonService _commonService =
                    scope.ServiceProvider.GetRequiredService<CommonService>();

                EmailService _emailService =
                    scope.ServiceProvider.GetRequiredService<EmailService>();

                using (var transaction = _haileckApplicationContext.Database.BeginTransaction())
                {
                    try
                    {
                        var parentData = _commonService.GetDataControlData(null, AppConstant.DataControl.DeactiveAccountInactiveForNumberOfDays, null);
                        var result = _haileckApplicationContext.DataControls.FirstOrDefault(e => e.Parent == parentData.Id);
                        var daysDeactive = Convert.ToInt32(result.Description);

                        var emailTypeData = _commonService.GetDataControlData(null, AppConstant.DataControl.EmailSubmitType, AppConstant.DataControl.EmailSubmitTypeData.InactiveForNumberOfDays);

                        var notOnlineUsers = _haileckApplicationContext.Users.Where(e => e.IsUserActive == true && e.IsActive == true &&
                        e.LastAccessDate.Value.AddDays(daysDeactive) <= DateTime.Now
                        && (!e.UserName.Contains("Admin") && !e.UserName.Contains("SuperAdmin"))).ToList();

                        foreach (var user in notOnlineUsers)
                        {
                            user.IsUserActive = false;
                            _haileckApplicationContext.Users.Update(user);


                            var save = _haileckApplicationContext.SaveChanges();
                            if (save > 0)
                            {
                                var latestSentEmail = _haileckApplicationContext.EmailSubmitteds.Where(e => e.UserId == user.Id &&
                                e.CreatedDate.Value.AddDays(daysDeactive) <= DateTime.Now).Count() >= 0;

                                //send email notification

                                if (latestSentEmail == true)
                                {
                                    var accountDeactive = (DateTime.Now - user.LastAccessDate.Value).Days;
                                    var sendEmail = _emailService.SendEmailToDeactiveUser(user, accountDeactive.ToString(), emailTypeData.Id);

                                    if (sendEmail != "Success")
                                    {
                                        transaction.Rollback();
                                    }
                                    else
                                    {
                                        transaction.Commit();
                                    }
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                    }
                }

                //1000 = 1 second, 10000 = 10 second, 36000 = 1 Hours, 86400 = 24 Hours
                //await Task.Delay(10000, stoppingToken);

                int catchHours = 12;
                try
                {
                    var startTime = DateTime.Now;
                    DateTime endTime;
                    if (!DateTime.TryParseExact("23:59:59", "HH:mm:ss", CultureInfo.InvariantCulture,DateTimeStyles.None, out endTime))
                    {
                        await Task.Delay(TimeSpan.FromHours(catchHours), stoppingToken);
                    }

                    TimeSpan time = new TimeSpan(endTime.Ticks - startTime.Ticks);

                    await Task.Delay(TimeSpan.FromMilliseconds(time.TotalMilliseconds), stoppingToken);
                }
                catch
                {
                    await Task.Delay(TimeSpan.FromHours(catchHours), stoppingToken);
                }
            }
        }
    }
}
