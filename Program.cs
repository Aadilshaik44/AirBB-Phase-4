using AirBB.Models.DataLayer;
using AirBB.Models.DomainModels;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<AirBnbContext>(opt =>
    opt.UseSqlite(builder.Configuration.GetConnectionString("AirBnbContext") ?? "Data Source=AirBnb.sqlite"));

// --- REGISTER GENERIC REPOSITORY ---
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

builder.Services.AddMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(20);
    options.Cookie.IsEssential = true;
});
builder.Services.AddHttpContextAccessor();

var app = builder.Build();

app.UseStaticFiles();
app.UseRouting();
app.UseSession();

app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();