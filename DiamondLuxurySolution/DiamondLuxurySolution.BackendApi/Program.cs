using DiamondLuxurySolution.Application.Repository;
using DiamondLuxurySolution.Application.Repository.About;
using DiamondLuxurySolution.Application.Repository.Discount;
using DiamondLuxurySolution.Application.Repository.Platform;
using DiamondLuxurySolution.Application.Repository.Promotion;
using DiamondLuxurySolution.Application.Repository.Slide;
using DiamondLuxurySolution.Application.Repository.User.Customer;
using DiamondLuxurySolution.Application.Repository.User.Staff;
using DiamondLuxurySolution.Data.EF;
using DiamondLuxurySolution.Data.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddDbContext<LuxuryDiamondShopContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("LuxuryDiamondDb"));
});
builder.Services.AddTransient<IPlatformRepo, PlatformRepo>();
builder.Services.AddTransient<IAboutRepo, AboutRepo>();
builder.Services.AddTransient<ISlideRepo, SlideRepo>();
builder.Services.AddTransient<IRoleRepo, RoleRepo>();
builder.Services.AddTransient<ICustomerRepo, CustomerRepo>();
builder.Services.AddTransient<IStaffRepo, StaffRepo>();
builder.Services.AddTransient<IPromotionRepo, PromotionRepo>();
builder.Services.AddTransient<IDiscountRepo, DiscountRepo>();


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
