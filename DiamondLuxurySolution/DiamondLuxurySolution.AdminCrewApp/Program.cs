using DiamondLuxurySolution.AdminCrewApp.Service.Customer;
using DiamondLuxurySolution.AdminCrewApp.Service.Category;
using DiamondLuxurySolution.AdminCrewApp.Service.IInspectionCertificate;
using DiamondLuxurySolution.AdminCrewApp.Service.Contact;
using DiamondLuxurySolution.AdminCrewApp.Service.Platform;
using DiamondLuxurySolution.AdminCrewApp.Service.Role;
using DiamondLuxurySolution.AdminCrewApp.Service.Staff;
using DiamondLuxurySolution.Data.EF;
using Microsoft.EntityFrameworkCore;
using DiamondLuxurySolution.AdminCrewApp.Service.Payment;
using DiamondLuxurySolution.AdminCrewApp.Service.Gem;
using DiamondLuxurySolution.AdminCrewApp.Service.SubGem;
using DiamondLuxurySolution.AdminCrewApp.Service.Material;
using DiamondLuxurySolution.AdminCrewApp.Service.Slide;
using DiamondLuxurySolution.AdminCrewApp.Service.Frame;
using DiamondLuxurySolution.AdminCrewApp.Service.Discount;
using DiamondLuxurySolution.AdminCrewApp.Service.Login;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using DiamondLuxurySolution.AdminCrewApp.Service.About;
using DiamondLuxurySolution.AdminCrewApp.Service.Promotion;
using DiamondLuxurySolution.AdminCrewApp.Service.GemPriceList;
using DiamondLuxurySolution.AdminCrewApp.Service.News;
using DiamondLuxurySolution.AdminCrewApp.Service.Warranty;
using DiamondLuxurySolution.AdminCrewApp.Service.KnowledgeNews;
using DiamondLuxurySolution.AdminCrewApp.Service.KnowledgeNewsCategoty;
using DiamondLuxurySolution.AdminCrewApp.Service.KnowledgeNewsCategory;
using DiamondLuxurySolution.AdminCrewApp.Service.Collection;
using DiamondLuxurySolution.AdminCrewApp.Service.Product;
using DiamondLuxurySolution.AdminCrewApp.Service.Order;
using DiamondLuxurySolution.AdminCrewApp.Service.Home;
using DiamondLuxurySolution.AdminCrewApp.Models;
using DinkToPdf.Contracts;
using DinkToPdf;
using DiamondLuxurySolution.AdminCrewApp.Service.WarrantyDetail;
using PdfSharp.Charting;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddHttpClient();
builder.Services.AddControllersWithViews();
builder.Services.AddTransient<ILoginApiService, LoginApiService>();
builder.Services.AddTransient<IHomeApiService, HomeApiService>();

builder.Services.AddTransient<IOrderApiService, OrderApiService>();

builder.Services.AddTransient<IProductApiService, ProductApiService>();

builder.Services.AddTransient<IRoleApiService, RoleApiService>();

builder.Services.AddTransient<IStaffApiService, StaffApiService>();

builder.Services.AddTransient<ICustomerApiService, CustomerApiService>();
builder.Services.AddTransient<INewsApiService, NewsApiService>();

builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

builder.Services.AddTransient<IPlatformApiService, PlatformApiService>();

builder.Services.AddTransient<IFrameApiService, FrameApiService>();

builder.Services.AddTransient<IPaymentApiService, PaymentApiService>();

builder.Services.AddTransient<IDiscountApiService, DiscountApiService>();

builder.Services.AddTransient<IPromotionApiService, PromotionApiService>();

builder.Services.AddTransient<IInspectionCertificateApiService, InspectionCertificateApiService>();

builder.Services.AddTransient<IContactApiService, ContactApiService>();

builder.Services.AddTransient<IGemApiService, GemApiService>();

builder.Services.AddTransient<IMaterialApiService, MaterialApiService>();

builder.Services.AddTransient<ISlideApiService, SlideApiService>();

builder.Services.AddTransient<IAboutApiService, AboutApiService>();

builder.Services.AddTransient<ICategoryApiService, CategoryApiService>();
builder.Services.AddTransient<ICollectionApiService, CollectionApiService>();
builder.Services.AddTransient<IInspectionCertificateApiService, InspectionCertificateApiService>();
builder.Services.AddTransient<ISubGemApiService, SubGemApiService>();

builder.Services.AddTransient<IGemPriceListApiService, GemPriceListApiService>();

builder.Services.AddTransient<IWarrantyApiService, WarrantyApiService>();

builder.Services.AddTransient<IKnowledgeNewsCategoryApiService, KnowledgeNewsCategoryApiService>();
builder.Services.AddTransient<IKnowLedgeNewsApiService, KnowledgeNewsApiService>();
builder.Services.AddTransient<IWarrantyDetailService, WarrantyDetailService>();
builder.Services.AddSingleton(typeof(IConverter), new SynchronizedConverter(new PdfTools()));

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromHours(1);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Login/Index/";
        options.AccessDeniedPath = "/Error/Unauthorized/";
    });
builder.Services.AddRazorPages()
    .AddRazorRuntimeCompilation();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // Set session timeout
    options.Cookie.HttpOnly = true; // Make the session cookie HTTP only
    options.Cookie.IsEssential = true; // Make the session cookie essential
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowCustomerDomain",
        builder =>
        {
            builder.WithOrigins("https://localhost:9001")
                   .AllowAnyHeader()
                   .AllowAnyMethod()
                   .AllowCredentials();
        });
});


builder.Services.AddSignalR();
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
app.UseAuthentication();
app.UseAuthorization();
app.UseSession();
StaffSessionHelper.Configure(app.Services.GetRequiredService<IHttpContextAccessor>());
app.UseEndpoints(endpoints =>
{
    endpoints.MapHub<AdminChatHub>("/adminChatHub");
});
app.UseStatusCodePages(async context =>
{
    if (context.HttpContext.Response.StatusCode == 404)
    {
        context.HttpContext.Response.Redirect("/Error/PageNotFound");
    }
    if (context.HttpContext.Response.StatusCode == 500)
    {
        context.HttpContext.Response.Redirect("/Error/InternalServerError");
    }
    if (context.HttpContext.Response.StatusCode == 401)
    {
        context.HttpContext.Response.Redirect("/Error/Unauthorized");
    }
});
app.UseCors("AllowCustomerDomain");
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Login}/{action=Index}/{id?}");

app.Run();
