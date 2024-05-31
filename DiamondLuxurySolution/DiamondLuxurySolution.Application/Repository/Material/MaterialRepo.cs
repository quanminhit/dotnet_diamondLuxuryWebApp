﻿using DiamondLuxurySolution.Data.EF;
using DiamondLuxurySolution.Data.Entities;
using DiamondLuxurySolution.ViewModel.Common;
using DiamondLuxurySolution.ViewModel.Models.About;
using DiamondLuxurySolution.ViewModel.Models.Contact;
using DiamondLuxurySolution.ViewModel.Models.Material;
using Microsoft.EntityFrameworkCore;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondLuxurySolution.Application.Repository.Material
{
    public class MaterialRepo : IMaterialRepo
    {
        private readonly LuxuryDiamondShopContext _context;
        public MaterialRepo(LuxuryDiamondShopContext context)
        {
            _context = context;
        }
        public async Task<ApiResult<bool>> CreateMaterial(CreateMaterialRequest request)
        {
            var errorList = new List<string>();
            if (string.IsNullOrEmpty(request.MaterialName))
            {
                errorList.Add("Vui lòng nhập tên nguyên liệu");
            }
            if (string.IsNullOrEmpty(request.Weight))
            {
                errorList.Add("Vui lòng nhập trọng lương nguyên liệu");
            }

            decimal weight = 0;
            try
            {
                weight = Convert.ToDecimal(request.Weight);

                if (weight <= 0)
                {
                    errorList.Add("trọng lượng > 0");
                }
            }
            catch (FormatException)
            {
                errorList.Add("Trọng lượng không hợp lệ");
            }

            decimal price = 0;
            try
            {
                price = Convert.ToDecimal(request.Price);

                if (price <= 0)
                {
                    errorList.Add("Giá tiền > 0");
                }
            }
            catch (FormatException)
            {
                errorList.Add("Giá tiền không hợp lệ");
            }
            if (errorList.Any())
            {
                return new ApiErrorResult<bool>("Không hợp lệ", errorList);
            }
            var material = new Data.Entities.Material
            {
                MaterialId = Guid.NewGuid(),
                MaterialName = request.MaterialName,
                Color = request.Color != null ? request.Color : "",
                Description = request.Description != null ? request.Description : "",
                Status = request.Status,
                EffectDate = request.EffectDate,
                Price = price,
                Weight = weight,
            };
            if (request.MaterialImage != null)
            {
                string firebaseUrl = await Utilities.Helper.ImageHelper.Upload(request.MaterialImage);
                material.MaterialImage = firebaseUrl;
            }
            else
            {
                material.MaterialImage = "";
            }
            _context.Materials.Add(material);
            await _context.SaveChangesAsync();
            return new ApiSuccessResult<bool>(true, "Success");
        }

        public async Task<ApiResult<bool>> DeleteMaterial(DeleteMaterialRequest request)
        {
            var material = await _context.Materials.FindAsync(request.MaterialId);
            if (material == null)
            {
                return new ApiErrorResult<bool>("Không tìm thấy nguyên liệu");
            }

            _context.Materials.Remove(material);
            await _context.SaveChangesAsync();
            return new ApiSuccessResult<bool>(false, "Success");
        }

        public async Task<ApiResult<MaterialVm>> GetMaterialById(Guid MaterialId)
        {
            var material = await _context.Materials.FindAsync(MaterialId);
            if (material == null)
            {
                return new ApiErrorResult<MaterialVm>("Không tìm thấy nguyên liệu");
            }
            var materialVm = new MaterialVm()
            {
                MaterialId = MaterialId,
                MaterialName = material.MaterialName,
                Color = material.Color,
                Weight = material.Weight,
                Price = material.Price,
                EffectDate = material.EffectDate,
                Description = material.Description,
                MaterialImage = material.MaterialImage,
                Status = material.Status,
            };
            return new ApiSuccessResult<MaterialVm>(materialVm, "Success");
        }

        public async Task<ApiResult<List<MaterialVm>>> GetAll()
        {
            var list = await _context.Materials.ToListAsync();
            var rs = list.Select(x => new MaterialVm()
            {
                MaterialId = x.MaterialId,
                MaterialName = x.MaterialName,
                Color = x.Color,
                Description = x.Description,
                MaterialImage = x.MaterialImage,
                Status = x.Status,
                EffectDate = x.EffectDate,
                Price = x.Price,
                Weight = x.Weight
                
            }).ToList();
            return new ApiSuccessResult<List<MaterialVm>>(rs);
        }

        public async Task<ApiResult<bool>> UpdateMaterial(UpdateMaterialRequest request)
        {
            var errorList = new List<string>();
            if (string.IsNullOrEmpty(request.MaterialName))
            {
                errorList.Add("Vui lòng nhập tên nguyên liệu");
            }
            if (string.IsNullOrEmpty(request.Weight.ToString()))
            {
                errorList.Add("Vui lòng nhập trọng lương nguyên liệu");
            }

            try
            {
                if (request.Weight <= 0)
                {
                    errorList.Add("Trọng lượng > 0");
                }
            }
            catch (FormatException)
            {
                errorList.Add("Trọng lượng không hợp lệ");
            }
            try
            {
                if (request.Price <= 0)
                {
                    errorList.Add("Gía tiền > 0");
                }
            }
            catch (FormatException)
            {
                errorList.Add("Gía tiền không hợp lệ");
            }
            if (errorList.Any())
            {
                return new ApiErrorResult<bool>("Không hợp lệ", errorList);
            }
            var material = await _context.Materials.FindAsync(request.MaterialId);
            if (material == null)
            {
                return new ApiErrorResult<bool>("Không tìm thấy nguyên liệu");
            }
            material.MaterialName = request.MaterialName;
            material.Description = request.Description != null ? request.Description : "";
            material.Color = request.Color != null ? request.Color : "";
            material.Weight = request.Weight;
            material.Price = request.Price;
            material.EffectDate = request.EffectDate;
            material.Status = request.Status;
            if (request.MaterialImage != null)
            {
                string firebaseUrl = await Utilities.Helper.ImageHelper.Upload(request.MaterialImage);
                material.MaterialImage = firebaseUrl;
            }
            else
            {
                material.MaterialImage = "";
            }

            await _context.SaveChangesAsync();
            return new ApiSuccessResult<bool>(true, "Success");
        }

        public async Task<ApiResult<PageResult<MaterialVm>>> ViewMaterialInCustomer(ViewMaterialRequest request)
        {
            var listMaterial = await _context.Materials.ToListAsync();
            if (request.Keyword != null)
            {
                listMaterial = listMaterial.Where(x => x.MaterialName.Contains(request.Keyword)).ToList();

            }
            listMaterial = listMaterial.Where(x => x.Status).OrderByDescending(x => x.MaterialName).ToList();

            int pageIndex = request.pageIndex ?? 1;

            var listPaging = listMaterial.ToPagedList(pageIndex, Utilities.Constants.Systemconstant.AppSettings.PAGE_SIZE).ToList();

            var listMaterialVm = listPaging.Select(x => new MaterialVm()
            {
                MaterialId = x.MaterialId,
                MaterialName = x.MaterialName,
                Description = x.Description,
                Color = x.Color,
                MaterialImage = x.MaterialImage,
                Status = x.Status,
                Weight = x.Weight,
                Price = x.Price,
                EffectDate = x.EffectDate
            }).ToList();
            var listResult = new PageResult<MaterialVm>()
            {
                Items = listMaterialVm,
                PageSize = Utilities.Constants.Systemconstant.AppSettings.PAGE_SIZE,
                TotalRecords = listMaterial.Count,
                PageIndex = pageIndex
            };
            return new ApiSuccessResult<PageResult<MaterialVm>>(listResult, "Success");
        }

        public async Task<ApiResult<PageResult<MaterialVm>>> ViewMaterialInManager(ViewMaterialRequest request)
        {
            var listMaterial = await _context.Materials.ToListAsync();
            if (request.Keyword != null)
            {
                listMaterial = listMaterial.Where(x => x.MaterialName.Contains(request.Keyword)).ToList();
            }
            listMaterial = listMaterial.OrderByDescending(x => x.MaterialName).ToList();

            int pageIndex = request.pageIndex ?? 1;

            var listPaging = listMaterial.ToPagedList(pageIndex, Utilities.Constants.Systemconstant.AppSettings.PAGE_SIZE).ToList();

            var listMaterialVm = listPaging.Select(x => new MaterialVm()
            {
                MaterialId = x.MaterialId,
                MaterialName = x.MaterialName,
                Description = x.Description,
                Color = x.Color,
                MaterialImage = x.MaterialImage,
                Status = x.Status,
                Weight = x.Weight,
                Price = x.Price,
                EffectDate = x.EffectDate
            }).ToList();
            var listResult = new PageResult<MaterialVm>()
            {
                Items = listMaterialVm,
                PageSize = Utilities.Constants.Systemconstant.AppSettings.PAGE_SIZE,
                TotalRecords = listMaterial.Count,
                PageIndex = pageIndex
            };
            return new ApiSuccessResult<PageResult<MaterialVm>>(listResult, "Success");
        }
    }
}
