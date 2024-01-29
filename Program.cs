using Pitstop.Helper;
using Pitstop.Models;
using Pitstop.Models.PitstopData;
using Pitstop.Models.Identity;
using Pitstop.Service;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(5); // Set timeout to 5 minutes
    options.Cookie.HttpOnly = true; // Prevent access to the cookie from client-side scripts
    options.Cookie.IsEssential = true; // Make the cookie essential for the session to function
});

builder.Services.AddDbContext<PitstopContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
    options.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
});
builder.Services.AddTransient<PitstopContext>();
builder.Services.AddScoped<CommonService>();

builder.Services.AddDataProtection()
    .PersistKeysToFileSystem(GetKeyRingDirectoryInfo(builder))
    .SetApplicationName("SharedCookiePitstop")
    .SetDefaultKeyLifetime(TimeSpan.FromDays(90)); // Set a key lifetime that makes sense for your scenario

builder.Services.ConfigureApplicationCookie(options =>
{
    options.Cookie.Name = ".AspNet.SharedCookiePitstop";
    options.Cookie.HttpOnly = true;
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    options.Cookie.IsEssential = true; // Make the session cookie essential for the application to function
    options.SlidingExpiration = true;
});

builder.Services.AddScoped<UtilityClass>();

builder.Services.AddIdentity<User, Role>()
    .AddUserStore<UserStore>()
    .AddRoleStore<RoleStore>()
    .AddDefaultTokenProviders();

builder.Services.AddAuthentication("Identity.Application");
builder.Services.ConfigureApplicationCookie(option =>
{
    option.Cookie.Name = ".AspNet.SharedCookiePitstop";
});

builder.Services.Configure<EmailConfiguration>(builder.Configuration.GetSection("EmailConfiguration"));
builder.Services.AddScoped<EmailService>();

builder.Services.AddHostedService<BackgroundWorkerService>();

builder.Services.AddScoped<ProjectFilter>();
builder.Services.AddScoped<Authorize>();
builder.Services.AddMvc(options =>
{
    options.Filters.Add<ProjectFilter>();
}).AddRazorRuntimeCompilation();

builder.Services.Configure<IdentityOptions>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequiredLength = 12;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireLowercase = true;
    options.Password.RequiredUniqueChars = 1;
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseAuthentication(); // Before routing
app.UseAuthorization();

app.UseRouting();

app.UseSession(); // Add session middleware if needed

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.UseSession();

app.Run();


static DirectoryInfo GetKeyRingDirectoryInfo(WebApplicationBuilder builder)
{
    string keyRingPath = builder.Configuration.GetSection("AppKeys").GetValue<string>("KeyRingPath");
    DirectoryInfo keyRingDirectoryInfo = new DirectoryInfo($"{keyRingPath}");
    if (keyRingDirectoryInfo.Exists)
    {
        return keyRingDirectoryInfo;
    }
    else
    {
        Directory.CreateDirectory(keyRingPath);
        return new DirectoryInfo($"{keyRingPath}");
    }
    throw new Exception($"key ring path not found");
}