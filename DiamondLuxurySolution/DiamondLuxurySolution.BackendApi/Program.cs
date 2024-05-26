using DiamondLuxurySolution.Application.Repository;
using DiamondLuxurySolution.Application.Repository.About;
using DiamondLuxurySolution.Application.Repository.Discount;
using DiamondLuxurySolution.Application.Repository.Gem;
using DiamondLuxurySolution.Application.Repository.InspectionCertificate;
using DiamondLuxurySolution.Application.Repository.Material;
using DiamondLuxurySolution.Application.Repository.MaterialPriceList;
using DiamondLuxurySolution.Application.Repository.GemPriceList;
using DiamondLuxurySolution.Application.Repository.News;
using DiamondLuxurySolution.Application.Repository.Platform;
using DiamondLuxurySolution.Application.Repository.Promotion;
using DiamondLuxurySolution.Application.Repository.Slide;
using DiamondLuxurySolution.Application.Repository.User.Customer;
using DiamondLuxurySolution.Application.Repository.User.Staff;
using DiamondLuxurySolution.Application.Repository.Warehouse;
using DiamondLuxurySolution.Data.EF;
using DiamondLuxurySolution.Data.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
<<<<<<< HEAD
using DiamondLuxurySolution.Application.Repository.Order;
using DiamondLuxurySolution.Application.Repository.Product;
=======
using DiamondLuxurySolution.Application.Repository.KnowledgeNewCatagory;
using DiamondLuxurySolution.Application.Repository.Category;
using DiamondLuxurySolution.Application.Repository.Collection;
using DiamondLuxurySolution.Application.Repository.Warranty;
using DiamondLuxurySolution.Application.Repository.KnowledgeNews;

>>>>>>> 59348e02b106350021dbba36fe0bb84fc3d839e4

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddDbContext<LuxuryDiamondShopContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("LuxuryDiamondDb"));
});

builder.Services.AddControllersWithViews()
    .AddNewtonsoftJson(options =>
    options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
);
builder.Services.AddTransient<IPlatformRepo, PlatformRepo>();
builder.Services.AddTransient<IAboutRepo, AboutRepo>();
builder.Services.AddTransient<ISlideRepo, SlideRepo>();
builder.Services.AddTransient<IRoleRepo, RoleRepo>();
builder.Services.AddTransient<ICustomerRepo, CustomerRepo>();
builder.Services.AddTransient<IStaffRepo, StaffRepo>();
builder.Services.AddTransient<IPromotionRepo, PromotionRepo>();
builder.Services.AddTransient<IDiscountRepo, DiscountRepo>();
builder.Services.AddTransient<IGemRepo, GemRepo>();
builder.Services.AddTransient<IInspectionCertificateRepo, InspectionCertificateRepo>();
builder.Services.AddTransient<IMaterialRepo, MaterialRepo>();
builder.Services.AddTransient<IMaterialPriceListRepo, MaterialPriceListRepo>();
builder.Services.AddTransient<INewsRepo, NewsRepo>();
builder.Services.AddTransient<IGemPriceListRepo, GemPriceListRepo>();
builder.Services.AddTransient<IWarehouseRepo, WarehouseRepo>();
<<<<<<< HEAD
builder.Services.AddTransient<IOrderRepo, OrderRepo>();
builder.Services.AddTransient<IProductRepo, ProductRepo>();


=======
builder.Services.AddTransient<IKnowledgeNewCatagoryRepo, KnowledgeNewCatagoryRepo>();
builder.Services.AddTransient<ICategoryRepo, CategoryRepo>();
builder.Services.AddTransient<ICollectionRepo, CollectionRepo>();
builder.Services.AddTransient<IWarrantyRepo, WarrantyRepo>();
builder.Services.AddTransient<IKnowledgeNewsRepo, KnowledgeNewsRepo>();
>>>>>>> 59348e02b106350021dbba36fe0bb84fc3d839e4

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddIdentity<AppUser, AppRole>()
        .AddEntityFrameworkStores<LuxuryDiamondShopContext>()
        .AddDefaultTokenProviders();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
