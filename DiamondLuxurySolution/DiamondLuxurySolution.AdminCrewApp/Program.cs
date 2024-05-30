<<<<<<< HEAD
using DiamondLuxurySolution.AdminCrewApp.Service.Category;
=======
<<<<<<< HEAD
using DiamondLuxurySolution.AdminCrewApp.Service.IInspectionCertificate;
=======
using DiamondLuxurySolution.AdminCrewApp.Service.Contact;
>>>>>>> 953636842e7bfe0c8626a339d1306a738b20037e
>>>>>>> c9e6e6aecd389dfaa5f320bedb9e8090e6d19fb0
using DiamondLuxurySolution.AdminCrewApp.Service.Platform;
using DiamondLuxurySolution.AdminCrewApp.Service.Role;
using DiamondLuxurySolution.Data.EF;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddHttpClient();
builder.Services.AddControllersWithViews();
builder.Services.AddTransient<IRoleApiService, RoleApiService>();

builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.AddTransient<IPlatformApiService, PlatformApiService>();
<<<<<<< HEAD
builder.Services.AddTransient<ICategoryApiService, CategoryApiService>();
=======
<<<<<<< HEAD
builder.Services.AddTransient<IInspectionCertificateApiService, InspectionCertificateApiService>();
=======
builder.Services.AddTransient<IContactApiService, ContactApiService>();
>>>>>>> 953636842e7bfe0c8626a339d1306a738b20037e
>>>>>>> c9e6e6aecd389dfaa5f320bedb9e8090e6d19fb0
builder.Services.AddDbContext<LuxuryDiamondShopContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("eShopSolutionDb"));
});
builder.Services.AddRazorPages()
    .AddRazorRuntimeCompilation();

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

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
