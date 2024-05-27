﻿using DiamondLuxurySolution.Data.EF;
using DiamondLuxurySolution.Data.Entities;
using DiamondLuxurySolution.ViewModel.Common;
using DiamondLuxurySolution.ViewModel.Models;
using DiamondLuxurySolution.ViewModel.Models.Category;
using DiamondLuxurySolution.ViewModel.Models.Collection;
using DiamondLuxurySolution.ViewModel.Models.Gem;
using DiamondLuxurySolution.ViewModel.Models.InspectionCertificate;
using DiamondLuxurySolution.ViewModel.Models.Material;
using DiamondLuxurySolution.ViewModel.Models.Product;
using DiamondLuxurySolution.ViewModel.Models.Warehouse;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondLuxurySolution.Application.Repository.Collection
{
    public class CollectionRepo : ICollectionRepo
    {
        private readonly LuxuryDiamondShopContext _context;
        public CollectionRepo(LuxuryDiamondShopContext context)
        {
            _context = context;
        }
        public async Task<ApiResult<bool>> CreateCollection(CreateCollectionRequest request)
        {
            if (string.IsNullOrEmpty(request.CollectionName))
            {
                return new ApiErrorResult<bool>("Vui lòng nhập tên bộ sưu tập");
            }
            string collectionId = await GenerateUniqueCollectionIdAsync();
            var listProductCollection = new List<ProductsCollection>();
            if (request.ListProductId.Count > 0)
            {
                foreach (var x in request.ListProductId)
                {
                    var productCollection = new ProductsCollection
                    {
                        CollectionId = collectionId,
                        ProductId = x,
                    };
                    listProductCollection.Add(productCollection);
                    _context.ProductsCollections.Add(productCollection);
                }
            }
            var collection = new DiamondLuxurySolution.Data.Entities.Collection
            {
                CollectionId = collectionId,
                CollectionName = request.CollectionName,
                Status = request.Status,
                ProductsCollections = listProductCollection,
                Thumbnail = !string.IsNullOrWhiteSpace(request.Thumbnail?.ToString())
                      ? await DiamondLuxurySolution.Utilities.Helper.ImageHelper.Upload(request.Thumbnail) : "",
                Description = !string.IsNullOrWhiteSpace(request.Description) ? request.Description : ""
            };
            _context.Collections.Add(collection);
            await _context.SaveChangesAsync();
            return new ApiSuccessResult<bool>(true, "Success");
        }



        #region Id tự tạo
        public async Task<string> GenerateUniqueCollectionIdAsync()
        {
            string newId;
            bool exists;
            Random rd = new Random();
            do
            {
                newId = "C" + rd.Next(0, 9) + rd.Next(0, 9) + rd.Next(0, 9) +
                    rd.Next(0, 9) + rd.Next(0, 9) + rd.Next(0, 9) + rd.Next(0, 9) + rd.Next(0, 9) + rd.Next(0, 9);
                exists = await _context.Collections.AnyAsync(ic => ic.CollectionId == newId);
            } while (exists);
            return newId;
        }
        #endregion
        public async Task<ApiResult<bool>> DeleteCollection(DeleteCollectionRequest request)
        {
            var collection = await _context.Collections.FindAsync(request.CollectionId);
            if (collection == null)
            {
                return new ApiErrorResult<bool>("Không tìm thấy bộ sưu tập");
            }
            _context.Collections.Remove(collection);
            await _context.SaveChangesAsync();
            return new ApiSuccessResult<bool>(false, "Success");
        }

        public async Task<ApiResult<CollectionVm>> GetCollectionById(string CollectionId)
        {
            var collection = await _context.Collections.FindAsync(CollectionId);
            if (collection == null)
            {
                return new ApiErrorResult<CollectionVm>("Không tìm thấy bộ sưu tập");
            }
            var listProductsCollection = _context.ProductsCollections.Where(x => x.CollectionId == collection.CollectionId).ToList();

            var listProductVm = new List<ProductVm>();
            foreach (var item in listProductsCollection)
            {
                try
                {
                    var product = await _context.Products
                        .Include(p => p.Images)
                        .Include(p => p.SubGemDetails)
                            .ThenInclude(sg => sg.SubGem)
                        .Include(p => p.Category)
                        .Include(p => p.InspectionCertificate)
                        .Include(p => p.Material)
                        .Include(p => p.Gem)
                        .Include(p => p.Images)
                        .Include(p => p.WareHouse)
                        .FirstOrDefaultAsync(p => p.ProductId == item.ProductId);

                    if (product == null)
                    {
                        return new ApiErrorResult<CollectionVm>("Không tìm thấy sản phẩm");
                    }
                    var productVms = new ProductVm
                    {
                        ProductId = product.ProductId,
                        ProductName = product.ProductName,
                        Description = product.Description,
                        ProductThumbnail = product.ProductThumbnail,
                        IsHome = product.IsHome,
                        IsSale = product.IsSale,
                        ProcessingPrice = (decimal)product.Category.CategoryPriceProcessing,
                        PercentSale = product.PercentSale,
                        Status = product.Status,
                        CategoryVm = new CategoryVm
                        {
                            CategoryId = product.Category.CategoryId,
                            CategoryName = product.Category.CategoryName,
                            CategoryType = product.Category.CategoryType,
                            CategoryImage = product.Category.CategoryImage,
                            CategoryPriceProcessing = product.Category.CategoryPriceProcessing,
                            Status = product.Category.Status
                        },


                        Quantity = product.Quantity,
                        GemVms = new GemVm
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

                        WareHouseVms = new WarehouseVm
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
                    if (product.Material != null)
                    {
                        productVms.MaterialVms = new MaterialVm
                        {
                            MaterialId = product.Material.MaterialId,
                            MaterialName = product.Material.MaterialName,
                            Color = product.Material.Color,
                            Description = product.Material.Description,
                            Status = product.Material.Status,
                            MaterialImage = product.Material.MaterialImage,
                            Weight = product.Material.Weight,
                        };
                    }
                    if (product.InspectionCertificate != null)
                    {
                        productVms.InspectionCertificateVm = new InspectionCertificateVm
                        {
                            InspectionCertificateId = product.InspectionCertificate.InspectionCertificateId,
                            InspectionCertificateName = product.InspectionCertificate.InspectionCertificateName,
                            Logo = product.InspectionCertificate.Logo,
                            DateGrading = product.InspectionCertificate.DateGrading,
                            Status = product.InspectionCertificate.Status
                        };
                    }
                    listProductVm.Add(productVms);
                }
                catch (Exception e)
                {
                    return new ApiErrorResult<CollectionVm>(e.Message);
                }
            }
            var collectionVm = new CollectionVm()
            {
                CollectionId = CollectionId,
                CollectionName = collection.CollectionName,
                Thumbnail = collection.Thumbnail,
                Status = collection.Status,
                Description = collection.Description,
                ListProducts = listProductVm
            };
            return new ApiSuccessResult<CollectionVm>(collectionVm, "Success");
        }

        public async Task<ApiResult<bool>> UpdateCollection(UpdateCollectionRequest request)
        {
            var collection = await _context.Collections.FindAsync(request.CollectionId);
            if (collection == null)
            {
                return new ApiErrorResult<bool>("Không tìm thấy bộ sưu tập");
            }
            if (string.IsNullOrEmpty(request.CollectionName))
            {
                return new ApiErrorResult<bool>("Vui lòng nhập tên bộ sưu tập");
            }
            if (request.ListProductsAdd.Count > 0 || request.ListProductsRemove.Count > 0 || request.ProductId != null)
            {
                var productCollection = _context.ProductsCollections.Where(x => x.CollectionId == request.CollectionId);
                if (productCollection.Any())
                {
                    List<Data.Entities.Product> existingProduct = new List<Data.Entities.Product>();
                    //Tim list product hien tai trong collection
                    foreach (var x in productCollection)
                    {
                        var product = await _context.Products.FindAsync(x.ProductId);
                        if (product != null)
                        {
                            existingProduct.Add(product);
                        }
                    }
                    //Xoa 1 product khoi collection
                    var productRemove = productCollection.SingleOrDefault(p => p.ProductId == request.ProductId);
                    if(productRemove != null)
                    {
                        _context.ProductsCollections.Remove(productRemove);
                    }

                    //Khi xoa 1 luot product khoi collection
                    if (request.ListProductsRemove.Count > 0)
                    {
                        var productsDeleteFromCollection = productCollection
                        .Where(pc => existingProduct.Any(p => p.ProductId == pc.ProductId)).ToList();
                        if (productsDeleteFromCollection.Any())
                        {
                            // Xoa cac product da xoa tu collection
                            _context.ProductsCollections.RemoveRange(productsDeleteFromCollection);
                        }
                    }
                    //Khi them product vao collection
                    if (request.ListProductsAdd.Count > 0)
                    {
                        foreach (var product in request.ListProductsAdd)
                        {
                            // Kiem tra product co ton tai trong collection khong roi moi add
                            if (!existingProduct.Contains(product))
                            {
                                _context.ProductsCollections.Add(new ProductsCollection
                                {
                                    CollectionId = request.CollectionId,
                                    ProductId = product.ProductId
                                });
                            }
                            else
                            {
                                return new ApiErrorResult<bool>("Sản phẩm đã tồn tại trong collection rồi");
                            }
                        }
                    }
                }
            }

            collection.Thumbnail = !string.IsNullOrWhiteSpace(request.Thumbnail?.ToString())
                 ? await DiamondLuxurySolution.Utilities.Helper.ImageHelper.Upload(request.Thumbnail) : "";
            collection.CollectionName = request.CollectionName;
            collection.Status = request.Status;
            collection.Description = !string.IsNullOrWhiteSpace(request.Description) ? request.Description : "";
            await _context.SaveChangesAsync();
            return new ApiSuccessResult<bool>(true, "Success");
        }


        public async Task<ApiResult<PageResult<CollectionVm>>> ViewCollectionInPaging(ViewCollectionRequest request)
        {
            var listCollection = await _context.Collections.ToListAsync();
            if (request.Keyword != null)
            {
                listCollection = listCollection.Where(x => x.CollectionName.Contains(request.Keyword)).ToList();

            }
            listCollection = listCollection.Where(x => x.Status).OrderByDescending(x => x.CollectionName).ToList();

            int pageIndex = request.pageIndex ?? 1;

            var listPaging = listCollection.ToPagedList(pageIndex, DiamondLuxurySolution.Utilities.Constants.Systemconstant.AppSettings.PAGE_SIZE).ToList();

            var listProductVm = new List<ProductVm>();
            var listCollectionVm = new List<CollectionVm>();
            foreach (var collection in listPaging)
            {
                var listProductsCollection = _context.ProductsCollections.Where(x => x.CollectionId == collection.CollectionId).ToList();
                foreach (var item in listProductsCollection)
                {
                    try
                    {
                        var product = await _context.Products
                            .Include(p => p.Images)
                            .Include(p => p.SubGemDetails)
                                .ThenInclude(sg => sg.SubGem)
                            .Include(p => p.Category)
                            .Include(p => p.InspectionCertificate)
                            .Include(p => p.Material)
                            .Include(p => p.Gem)
                            .Include(p => p.Images)
                            .Include(p => p.WareHouse)
                            .FirstOrDefaultAsync(p => p.ProductId == item.ProductId);

                        if (product == null)
                        {
                            return new ApiErrorResult<PageResult<CollectionVm>>("Không tìm thấy sản phẩm");
                        }
                        var productVms = new ProductVm
                        {
                            ProductId = product.ProductId,
                            ProductName = product.ProductName,
                            Description = product.Description,
                            ProductThumbnail = product.ProductThumbnail,
                            IsHome = product.IsHome,
                            IsSale = product.IsSale,
                            ProcessingPrice = (decimal)product.Category.CategoryPriceProcessing,
                            PercentSale = product.PercentSale,
                            Status = product.Status,
                            CategoryVm = new CategoryVm
                            {
                                CategoryId = product.Category.CategoryId,
                                CategoryName = product.Category.CategoryName,
                                CategoryType = product.Category.CategoryType,
                                CategoryImage = product.Category.CategoryImage,
                                CategoryPriceProcessing = product.Category.CategoryPriceProcessing,
                                Status = product.Category.Status
                            },


                            Quantity = product.Quantity,
                            GemVms = new GemVm
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

                            WareHouseVms = new WarehouseVm
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
                        if (product.Material != null)
                        {
                            productVms.MaterialVms = new MaterialVm
                            {
                                MaterialId = product.Material.MaterialId,
                                MaterialName = product.Material.MaterialName,
                                Color = product.Material.Color,
                                Description = product.Material.Description,
                                Status = product.Material.Status,
                                MaterialImage = product.Material.MaterialImage,
                                Weight = product.Material.Weight,
                            };
                        }
                        if (product.InspectionCertificate != null)
                        {
                            productVms.InspectionCertificateVm = new InspectionCertificateVm
                            {
                                InspectionCertificateId = product.InspectionCertificate.InspectionCertificateId,
                                InspectionCertificateName = product.InspectionCertificate.InspectionCertificateName,
                                Logo = product.InspectionCertificate.Logo,
                                DateGrading = product.InspectionCertificate.DateGrading,
                                Status = product.InspectionCertificate.Status
                            };
                        }
                        listProductVm.Add(productVms);
                    }
                    catch (Exception e)
                    {
                        return new ApiErrorResult<PageResult<CollectionVm>>(e.Message);
                    }
                }
                var CollectionVm = new CollectionVm()
                {
                    CollectionId = collection.CollectionId,
                    CollectionName = collection.CollectionName,
                    Description = collection.Description,
                    Thumbnail = collection.Thumbnail,
                    Status = collection.Status,
                    ListProducts = listProductVm
                };
                listCollectionVm.Add(CollectionVm);
            }
            var listResult = new PageResult<CollectionVm>()
            {
                Items = listCollectionVm,
                PageSize = DiamondLuxurySolution.Utilities.Constants.Systemconstant.AppSettings.PAGE_SIZE,
                TotalRecords = listCollection.Count,
                PageIndex = pageIndex
            };
            return new ApiSuccessResult<PageResult<CollectionVm>>(listResult, "Success");
        }

    }
}
