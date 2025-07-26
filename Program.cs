using ERManager.Data;
using ERManager.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Net.Sockets;
using System.Diagnostics;
using System.Net.NetworkInformation;

var builder = WebApplication.CreateBuilder(args);

// Configure Entity Framework with SQLite
builder.Services.AddDbContext<ERManagerContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("ERManagerContext")
                      ?? throw new InvalidOperationException("Connection string 'ERManagerContext' not found.")));

// Configure Identity
builder.Services.AddIdentity<User, IdentityRole>(options =>
{
    options.Password.RequireDigit = false;
    options.Password.RequiredLength = 6;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireLowercase = false;

    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(40);
    options.Lockout.MaxFailedAccessAttempts = 10;
    options.Lockout.AllowedForNewUsers = true;

    options.User.RequireUniqueEmail = false;
    options.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";

    options.SignIn.RequireConfirmedAccount = false;
})
.AddEntityFrameworkStores<ERManagerContext>()
.AddDefaultTokenProviders();

builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();
builder.Services.AddSingleton<IEmailSender, NoOpEmailSender>();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Identity/Account/Login";
    options.LogoutPath = "/Identity/Account/Logout";
    options.AccessDeniedPath = "/Identity/Account/AccessDenied";
    options.SlidingExpiration = true;
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", builder =>
    {
        builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
    });
});

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromHours(8);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

builder.Services.AddHttpContextAccessor();

// Dynamic port handling (HTTP only)
int httpPort = 5000;
while (IsPortInUse(httpPort)) httpPort++;

builder.WebHost.UseUrls($"http://0.0.0.0:{httpPort}");

var app = builder.Build();

// Seed roles and admin user
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    await SeedRolesAndAdminUser(services);
}

// Configure middleware
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts(); // optional, still safe
}

//app.UseHttpsRedirection(); ← removed HTTPS
app.UseStaticFiles();
app.UseRouting();
app.UseSession();
app.UseCors("AllowAll");
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages();

// Apply any pending migrations
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<ERManagerContext>();
    await dbContext.Database.MigrateAsync();
}

// Optional: open browser on startup (still HTTP)
OpenBrowser($"http://0.0.0.0:{httpPort}");

app.Run();

// Seed roles and default admin user
static async Task SeedRolesAndAdminUser(IServiceProvider serviceProvider)
{
    var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var userManager = serviceProvider.GetRequiredService<UserManager<User>>();

    if (roleManager == null || userManager == null)
        throw new Exception("RoleManager or UserManager not resolved.");

    string[] roleNames = { "Admin", "User" };
    foreach (var roleName in roleNames)
    {
        if (!await roleManager.RoleExistsAsync(roleName))
            await roleManager.CreateAsync(new IdentityRole(roleName));
    }

    if (!userManager.Users.Any())
    {
        var defaultUser = new User { UserName = "admin", Email = "admin@example.com" };
        var result = await userManager.CreateAsync(defaultUser, "Admin@123");
        if (result.Succeeded)
            await userManager.AddToRoleAsync(defaultUser, "Admin");
    }
}

// Port checker
static bool IsPortInUse(int port)
{
    try
    {
        using var listener = new TcpListener(IPAddress.Loopback, port);
        listener.Start();
        listener.Stop();
        return false;
    }
    catch (SocketException)
    {
        return true;
    }
}

// Launch browser
static void OpenBrowser(string url)
{
    var localIp = GetLocalIPAddress();
    if (!string.IsNullOrEmpty(localIp))
        url = url.Replace("0.0.0.0", localIp);

    try
    {
        Process.Start(new ProcessStartInfo
        {
            FileName = url,
            UseShellExecute = true
        });
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Failed to open browser. Please navigate to {url} manually. Error: {ex.Message}");
    }
}

// Local IP fetcher
static string GetLocalIPAddress()
{
    if (NetworkInterface.GetIsNetworkAvailable())
    {
        var host = Dns.GetHostEntry(Dns.GetHostName());
        foreach (var ip in host.AddressList)
        {
            if (ip.AddressFamily == AddressFamily.InterNetwork)
                return ip.ToString();
        }
    }

    return "localhost";
}
