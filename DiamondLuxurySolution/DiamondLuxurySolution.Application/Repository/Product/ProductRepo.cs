﻿using DiamondLuxurySolution.Data.EF;
using DiamondLuxurySolution.Data.Entities;
using DiamondLuxurySolution.ViewModel.Common;
using DiamondLuxurySolution.ViewModel.Models;
using DiamondLuxurySolution.ViewModel.Models.Category;
using DiamondLuxurySolution.ViewModel.Models.Gem;
using DiamondLuxurySolution.ViewModel.Models.InspectionCertificate;
using DiamondLuxurySolution.ViewModel.Models.Material;
using DiamondLuxurySolution.ViewModel.Models.Platform;
using DiamondLuxurySolution.ViewModel.Models.Product;
using DiamondLuxurySolution.ViewModel.Models.Warehouse;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PagedList;
using System;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace DiamondLuxurySolution.Application.Repository.Product
{
    public class ProductRepo : IProductRepo
    {

        private readonly LuxuryDiamondShopContext _context;
        private readonly UserManager<AppUser> _userManager;
        public ProductRepo(LuxuryDiamondShopContext context, UserManager<AppUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
        public async Task<ApiResult<bool>> CreateProduct(CreateProductRequest request)
        {
            try
            {

                if (string.IsNullOrEmpty(request.ProductName))
                {
                    return new ApiErrorResult<bool>("Vui lòng nhập tên sản phẩm");
                }
                if (request.GemId == null)
                {
                    return new ApiErrorResult<bool>("Sản phẩm cần phải có kim cương");
                }
                List<string> errorList = new List<string>();
                if (request.ProductThumbnail == null)
                {
                    errorList.Add("Sản phẩm phải có ảnh đại diện");
                }

                if (request.WareHouseId == null)
                {
                    errorList.Add("Sản phẩm cần phải có kho lưu trữ");
                }
                if (request.CategoryId == null)
                {
                    errorList.Add("Sản phẩm cần phải có loại");
                }
                if (request.Quantity == null || request.Quantity <= 0)
                {
                    errorList.Add("Sản phẩm cần có số lượng ít nhất là 1");
                }
                if (errorList.Any())
                {
                    return new ApiErrorResult<bool>("Lỗi thông tin", errorList);
                }
                Random rd = new Random();
                string ProductId = "P" + rd.Next(0, 9) + rd.Next(0, 9) + rd.Next(0, 9) + rd.Next(0, 9) + rd.Next(0, 9) + rd.Next(0, 9);
                // Process Product gem
                decimal totalPriceGem = 0;

                var gem = await _context.Gems.FindAsync(request.GemId);
                if (gem == null)
                {
                    return new ApiErrorResult<bool>("Không tìm thấy kim cương");
                }

                var GemPriceList = _context.GemPriceLists.Where(x => x.GemId == request.GemId);
                if (GemPriceList == null)
                {
                    return new ApiErrorResult<bool>("Không tìm thấy kim cương trong bảng giá kim cương");
                }
                foreach (var item in GemPriceList)
                {
                    if (item.effectDate.Date == DateTime.Now.Date)
                    {
                        totalPriceGem += (decimal)item.Price;
                    }
                }
                if (errorList.Any())
                {
                    return new ApiErrorResult<bool>("Lỗi tìm kiếm", errorList);
                }
                // Process Material NEED FIX
                /*decimal totalMaterialPrice = 0;
                if (request.MaterialId != null)
                {
                    var materialList = _context.MaterialPriceLists.Where(x => x.MaterialId == request.MaterialId);
                    if (materialList == null)
                    {
                        return new ApiErrorResult<bool>($"Không tìm nguyên liệu");
                    }
                    foreach (var item in materialList)
                    {
                        if (item.effectDate.Date == DateTime.Now.Date)
                        {
                            totalMaterialPrice += (decimal)item.SellPrice;
                        }
                    }
                    if (errorList.Any())
                    {
                        return new ApiErrorResult<bool>("Lỗi tìm kiếm", errorList);
                    }
                }*/
                // Process SubGemPrice
                decimal totalSubGemPrice = 0;
                if (request.ListSubGems != null && request.ListSubGems.Count > 0)
                {
                    foreach (var subGemSupport in request.ListSubGems)
                    {
                        var subGem = await _context.SubGems.FindAsync(subGemSupport.SubGemId);
                        if (subGem == null)
                        {
                            return new ApiErrorResult<bool>($"Không tìm thấy kim cương phụ");
                        }
                        var subGemDetail = new SubGemDetail()
                        {
                            ProductId = ProductId,
                            SubGemId = subGemSupport.SubGemId,
                            Quantity = subGemSupport.Quantity,
                        };
                        totalSubGemPrice += subGem.SubGemPrice * subGemSupport.Quantity;
                        _context.SubGemDetail.Add(subGemDetail);
                    }
                }


                // Process Image Of Product
                if (request.Images.Any() && request.Images.Count > 0)
                {
                    foreach (var item in request.Images)
                    {
                        string firebaseUrl = await DiamondLuxurySolution.Utilities.Helper.ImageHelper.Upload(item);
                        var Image = new Image()
                        {
                            ImagePath = firebaseUrl,
                            Description = $"Hình ảnh của sản phẩm {request.ProductName}",
                            ProductId = ProductId
                        };
                        _context.Images.Add(Image);
                    }
                }
                // Process Category
                var category = await _context.Categories.FindAsync(request.CategoryId);
                if (category == null)
                {
                    return new ApiErrorResult<bool>("Không tìm thấy loại của sản phẩm");
                }
                // Process Thumbnail
                string thumbnailUrl = await DiamondLuxurySolution.Utilities.Helper.ImageHelper.Upload(request.ProductThumbnail);

                // Process totalPrice
                decimal OriginalPrice = (decimal)(totalPriceGem + totalSubGemPrice); // + them cai processing price cua product
                double percent = (double)request.PercentSale / 100;
                decimal SellingPrice = OriginalPrice - (OriginalPrice * (decimal)percent);

                // Save Product
                var product = new DiamondLuxurySolution.Data.Entities.Product()
                {
                    ProductId = ProductId,
                    ProductName = request.ProductName,
                    Description = string.IsNullOrEmpty(request.Description) ? string.Empty : request.Description,
                    ProductThumbnail = thumbnailUrl,
                    IsHome = request.IsHome,
                    IsSale = request.IsSale,
                    DateCreate = DateTime.Now,
                    DateModified = DateTime.Now,
                    OriginalPrice = OriginalPrice,
                    SellingPrice = SellingPrice,
                    SellingCount = 0,
                    Quantity = request.Quantity,
                    PercentSale = request.PercentSale,
                    CategoryId = request.CategoryId,
                    Status = request.Status,
                    WarehouseId = request.WareHouseId,
                    GemId = gem.GemId
                };
                _context.Products.Add(product);

                await _context.SaveChangesAsync();
                return new ApiSuccessResult<bool>(true, "Success");
            }
            catch (Exception e)
            {
                return new ApiErrorResult<bool>(e.Message);
            }
        }

        public async Task<ApiResult<bool>> DeleteProduct(DeleteProductRequest request)
        {
            var product = await _context.Products.FindAsync(request.ProductId);
            if (product == null)
            {
                return new ApiErrorResult<bool>("Không tìm thấy sản phẩm");
            }
            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
            return new ApiSuccessResult<bool>(true, "Success");
        }

        public async Task<ApiResult<ProductVm>> GetProductById(string ProductId)
        {
            try
            {
                var product = await _context.Products
                    .Include(p => p.Images)
                    .Include(p => p.SubGemDetails)
                        .ThenInclude(sg => sg.SubGem)
                    .Include(p => p.Category)
                    .Include(p => p.Gem)
                    .Include(p => p.Images)
                    .Include(p => p.WareHouse)
                    .FirstOrDefaultAsync(p => p.ProductId == ProductId);

                if (product == null)
                {
                    return new ApiErrorResult<ProductVm>("Không tìm thấy sản phẩm");
                }


                var productVms = new ProductVm
                {
                    ProductId = product.ProductId,
                    ProductName = product.ProductName,
                    Description = product.Description,
                    ProductThumbnail = product.ProductThumbnail,
                    IsHome = product.IsHome,
                    IsSale = product.IsSale,
                    PercentSale = product.PercentSale,
                    Status = product.Status,
                    Category = new CategoryVm
                    {
                        CategoryId = product.Category.CategoryId,
                        CategoryName = product.Category.CategoryName,
                        CategoryType = product.Category.CategoryType,
                        CategoryImage = product.Category.CategoryImage,
                        Status = product.Category.Status
                    },


                    Quantity = product.Quantity,
                    Gem = new GemVm
                    {
                        GemId = product.Gem.GemId,
                        GemName = product.Gem.GemName,
                        GemImage = product.Gem.GemImage,
                        AcquisitionDate = product.Gem.AcquisitionDate,
                        IsOrigin = product.Gem.IsOrigin,
                        Active = product.Gem.Active,
                        Fluoresence = product.Gem.Fluoresence,
                        Polish = product.Gem.Polish,
                        ProportionImage = product.Gem.ProportionImage,
                        Symetry = product.Gem.Symetry
                    },

                    WareHouse = new WarehouseVm
                    {
                        WarehouseId = product.WareHouse.WareHouseId,
                        Description = product.WareHouse.Description,
                        Location = product.WareHouse.Location,
                        WareHouseName = product.WareHouse.WareHouseName
                    }
                };
                if (product.Images != null)
                {
                    var imagePaths = product.Images.Select(x => x.ImagePath).ToList();
                    productVms.Images = imagePaths;
                }
                if (product.SubGemDetails != null)
                {
                    productVms.ListSubGems = product.SubGemDetails.Select(x => new SubGemSupportDTO
                    {
                        SubGemId = x.SubGemId,
                        Quantity = x.Quantity
                    }).ToList();
                }
              
                return new ApiSuccessResult<ProductVm>(productVms);
            }
            catch (Exception e)
            {
                return new ApiErrorResult<ProductVm>(e.Message);
            }
        }


        public async Task<ApiResult<bool>> UpdateProduct(UpdateProductRequest request)
        {
            try
            {
                if (string.IsNullOrEmpty(request.ProductName))
                {
                    return new ApiErrorResult<bool>("Vui lòng nhập tên sản phẩm");
                }
                if (request.GemId == null)
                {
                    return new ApiErrorResult<bool>("Sản phẩm cần phải có kim cương");
                }

                List<string> errorList = new List<string>();
                if (request.ProductThumbnail == null)
                {
                    errorList.Add("Sản phẩm phải có ảnh đại diện");
                }

                if (request.WareHouseId == null)
                {
                    errorList.Add("Sản phẩm cần phải có kho lưu trữ");
                }

                if (errorList.Any())
                {
                    return new ApiErrorResult<bool>("Lỗi thông tin", errorList);
                }

                var product = await _context.Products.FindAsync(request.ProductId);
                if (product == null)
                {
                    return new ApiErrorResult<bool>("Không tìm thấy sản phẩm");
                }

                // Process Product gem
                decimal totalPriceGem = 0;
                var gem = await _context.Gems.FindAsync(request.GemId);
                if (gem == null)
                {
                    return new ApiErrorResult<bool>("Không tìm thấy kim cương");
                }

                var GemPriceList = _context.GemPriceLists.Where(x => x.GemId == request.GemId);
                if (GemPriceList == null)
                {
                    return new ApiErrorResult<bool>("Không tìm thấy kim cương trong bảng giá kim cương");
                }

                foreach (var item in GemPriceList)
                {
                    if (item.effectDate.Date == DateTime.Now.Date)
                    {
                        totalPriceGem +=  (decimal)item.Price;
                    }
                }
                product.GemId = gem.GemId;
                if (errorList.Any())
                {
                    return new ApiErrorResult<bool>("Lỗi tìm kiếm", errorList);
                }

                // Process Material NEED FIX
                /*decimal totalMaterialPrice = 0;
                if (request.MaterialId != null)
                {
                    var materialList = _context.MaterialPriceLists.Where(x => x.MaterialId == request.MaterialId);
                    if (materialList == null)
                    {
                        return new ApiErrorResult<bool>("Không tìm nguyên liệu");
                    }

                    foreach (var item in materialList)
                    {
                        if (item.effectDate.Date == DateTime.Now.Date)
                        {
                            totalMaterialPrice += (decimal)item.SellPrice;
                        }
                    }

                    if (errorList.Any())
                    {
                        return new ApiErrorResult<bool>("Lỗi tìm kiếm", errorList);
                    }
                    product.MaterialId = request.MaterialId;
                }
                else
                {
                    product.MaterialId = null;
                }*/

                // Process SubGemPrice
                decimal totalSubGemPrice = 0;
                if (request.ListSubGems != null)
                {
                    var existingSubGems = _context.SubGemDetail.Where(x => x.ProductId == request.ProductId).ToList();

                    // Remove existing sub-gems if they are not in the new list
                    foreach (var existingSubGem in existingSubGems)
                    {
                        if (!request.ListSubGems.Any(x => x.SubGemId == existingSubGem.SubGemId))
                        {
                            _context.SubGemDetail.Remove(existingSubGem);
                        }
                    }

                    foreach (var subGemSupport in request.ListSubGems)
                    {
                        var subGem = await _context.SubGems.FindAsync(subGemSupport.SubGemId);
                        if (subGem == null)
                        {
                            return new ApiErrorResult<bool>("Không tìm thấy kim cương phụ");
                        }

                        var subGemDetail = existingSubGems.FirstOrDefault(x => x.SubGemId == subGemSupport.SubGemId);

                        if (subGemDetail == null)
                        {
                            subGemDetail = new SubGemDetail
                            {
                                ProductId = request.ProductId,
                                SubGemId = subGemSupport.SubGemId,
                                Quantity = subGemSupport.Quantity
                            };
                            _context.SubGemDetail.Add(subGemDetail);
                        }
                        else
                        {
                            subGemDetail.Quantity = subGemSupport.Quantity;
                        }

                        totalSubGemPrice += subGem.SubGemPrice * subGemSupport.Quantity;
                    }
                }
                else
                {
                    // If ListSubGems is null or empty, remove all existing sub-gems for the product
                    var existingSubGems = _context.SubGemDetail.Where(x => x.ProductId == request.ProductId).ToList();
                    _context.SubGemDetail.RemoveRange(existingSubGems);
                }

                // Process totalPrice
                decimal OriginalPrice = totalPriceGem + totalSubGemPrice;
                decimal SellingPrice = OriginalPrice - (OriginalPrice * request.PercentSale);

                // Process Image Of Product
                if (request.Images != null && request.Images.Any())
                {
                    var existingImages = _context.Images.Where(x => x.ProductId == request.ProductId).ToList();
                    _context.Images.RemoveRange(existingImages);

                    foreach (var item in request.Images)
                    {
                        string firebaseUrl = await DiamondLuxurySolution.Utilities.Helper.ImageHelper.Upload(item);
                        var image = new Image
                        {
                            ImagePath = firebaseUrl,
                            Description = $"Hình ảnh của sản phẩm {request.ProductName}",
                            ProductId = request.ProductId
                        };
                        _context.Images.Add(image);
                    }
                }
                else
                {
                    var existingImages = _context.Images.Where(x => x.ProductId == request.ProductId).ToList();
                    _context.Images.RemoveRange(existingImages);
                }

                // Process Category
                var category = await _context.Categories.FindAsync(request.CategoryId);
                if (category == null)
                {
                    return new ApiErrorResult<bool>("Không tìm thấy loại của sản phẩm");
                }
                product.CategoryId = category.CategoryId;

                // Process Thumbnail

                string thumbnailUrl = await DiamondLuxurySolution.Utilities.Helper.ImageHelper.Upload(request.ProductThumbnail);
                product.ProductThumbnail = thumbnailUrl;
                // Update Product
                product.ProductName = request.ProductName;
                product.Description = string.IsNullOrEmpty(request.Description) ? string.Empty : request.Description;
                product.IsHome = request.IsHome;
                product.IsSale = request.IsSale;
                product.DateModified = DateTime.Now;
                product.OriginalPrice = OriginalPrice;
                product.SellingPrice = SellingPrice;
                product.PercentSale = request.PercentSale;
                product.Status = request.Status;
                product.WarehouseId = request.WareHouseId;

                _context.Products.Update(product);

                await _context.SaveChangesAsync();
                return new ApiSuccessResult<bool>(true, "Cập nhật sản phẩm thành công");
            }
            catch (Exception e)
            {
                return new ApiErrorResult<bool>(e.Message);
            }
        }

        public async Task<ApiResult<PageResult<ProductVm>>> ViewProduct(ViewProductRequest request)
        {
            var listProduct = await _context.Products
                    .Include(p => p.Images)
                    .Include(p => p.SubGemDetails)
                        .ThenInclude(sg => sg.SubGem)
                    .Include(p => p.Category)
                    .Include(p => p.Gem)
                    .Include(p => p.Images)
                    .Include(p => p.WareHouse)
                                            .ToListAsync();

            if (!string.IsNullOrEmpty(request.Keyword))
            {
                listProduct = listProduct.Where(x => x.ProductName.Contains(request.Keyword, StringComparison.OrdinalIgnoreCase)
                                                   || x.Description.Contains(request.Keyword, StringComparison.OrdinalIgnoreCase)
                                                   || x.Gem.GemName.Contains(request.Keyword, StringComparison.OrdinalIgnoreCase)
                                                   || x.Category.CategoryName.Contains(request.Keyword, StringComparison.OrdinalIgnoreCase)
                                                   || x.WareHouse.WareHouseName.Contains(request.Keyword, StringComparison.OrdinalIgnoreCase)
                                                   || x.SubGemDetails.Any(sg => sg.SubGem.SubGemName.Contains(request.Keyword, StringComparison.OrdinalIgnoreCase))
                                                   || x.SubGemDetails.Any(sg => sg.SubGem.Description.Contains(request.Keyword, StringComparison.OrdinalIgnoreCase))).ToList();
            }

            var totalRecord = listProduct.Count();
            listProduct = listProduct.OrderBy(x => x.ProductName).ToList();

            int pageIndex = request.pageIndex ?? 1;
            int pageSize = DiamondLuxurySolution.Utilities.Constants.Systemconstant.AppSettings.PAGE_SIZE;
            var pagedProducts = listProduct.ToPagedList(pageIndex, pageSize).ToList();

            List<ProductVm> listResultVm = new List<ProductVm>();

            foreach (var item in pagedProducts)
            {
                var gem = item.Gem;
                var warehouse = item.WareHouse;
                var listSubGem = item.SubGemDetails.ToList();
                List<SubGemSupportDTO> listSubGemVm = listSubGem.Select(sg => new SubGemSupportDTO
                {
                    SubGemId = sg.SubGemId,
                    Quantity = sg.Quantity
                }).ToList();

                var productVms = new ProductVm
                {
                    ProductId = item.ProductId,
                    ProductName = item.ProductName,
                    Description = item.Description,
                    ProductThumbnail = item.ProductThumbnail,
                    IsHome = item.IsHome,
                    IsSale = item.IsSale,
                    PercentSale = item.PercentSale,
                    Status = item.Status,
                    Category = new CategoryVm
                    {
                        CategoryId = item.Category.CategoryId,
                        CategoryName = item.Category.CategoryName,
                        CategoryType = item.Category.CategoryType,
                        CategoryImage = item.Category.CategoryImage,
                        Status = item.Category.Status
                    },


                    Quantity = item.Quantity,
                    Gem = new GemVm
                    {
                        GemId = item.Gem.GemId,
                        GemName = item.Gem.GemName,
                        GemImage = item.Gem.GemImage,
                        AcquisitionDate = item.Gem.AcquisitionDate,
                        IsOrigin = item.Gem.IsOrigin,
                        Active = item.Gem.Active,
                        Fluoresence = item.Gem.Fluoresence,
                        Polish = item.Gem.Polish,
                        ProportionImage = item.Gem.ProportionImage,
                        Symetry = item.Gem.Symetry
                    },

                    WareHouse = new WarehouseVm
                    {
                        WarehouseId = item.WareHouse.WareHouseId,
                        Description = item.WareHouse.Description,
                        Location = item.WareHouse.Location,
                        WareHouseName = item.WareHouse.WareHouseName
                    }
                };
                if (item.Images != null)
                {
                    var imagePaths = item.Images.Select(x => x.ImagePath).ToList();
                    productVms.Images = imagePaths;
                }
                if (item.SubGemDetails != null)
                {
                    productVms.ListSubGems = item.SubGemDetails.Select(x => new SubGemSupportDTO
                    {
                        SubGemId = x.SubGemId,
                        Quantity = x.Quantity
                    }).ToList();
                }
               

                listResultVm.Add(productVms);
            }

            var listResult = new PageResult<ProductVm>
            {
                Items = listResultVm.Distinct().ToList(),
                PageSize = pageSize,
                TotalRecords = totalRecord,
                PageIndex = pageIndex
            };

            return new ApiSuccessResult<PageResult<ProductVm>>(listResult, "Success");
        }



    }
}
