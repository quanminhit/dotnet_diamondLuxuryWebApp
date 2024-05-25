﻿using Azure.Core;
using DiamondLuxurySolution.Data.EF;
using DiamondLuxurySolution.Data.Entities;
using DiamondLuxurySolution.ViewModel.Common;
using DiamondLuxurySolution.ViewModel.Models.Platform;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondLuxurySolution.Application.Repository.Platform
{
    public class PlatformRepo : IPlatformRepo
    {
        private readonly LuxuryDiamondShopContext _context;
        public PlatformRepo(LuxuryDiamondShopContext context)
        {
            _context = context;
        }
        public async Task<ApiResult<bool>> CreatePlatform(CreatePlatformRequest request)
        {
            var errorList = new List<string>();
            if (string.IsNullOrEmpty(request.PlatformName))
            {
                errorList.Add("Vui lòng nhập tên nền tảng");
                //return new ApiErrorResult<bool>("Vui lòng nhập tên nền tảng");
            }
            if (string.IsNullOrEmpty(request.PlatformUrl))
            {
                errorList.Add("Vui lòng nhập Url của nền tảng");
                //return new ApiErrorResult<bool>("Vui lòng nhập Url của nền tảng");
            }
            if (request.PlatformLogo == null)
            {
                errorList.Add("Vui lòng gắn Logo");
                //return new ApiErrorResult<bool>("Vui lòng gắn Logo");
            }
            if (errorList.Any())
            {
                return new ApiErrorResult<bool>("Không hợp lệ", errorList);
            }
            string firebaseUrl = await DiamondLuxurySolution.Utilities.Helper.ImageHelper.Upload(request.PlatformLogo);
            var platform = new DiamondLuxurySolution.Data.Entities.Platform
            {
                PlatformName = request.PlatformName,
                PlatformLogo = firebaseUrl,
                PlatformUrl = request.PlatformUrl,
                Status = request.Status,
            };
            _context.Platforms.Add(platform);
            await _context.SaveChangesAsync();
            return new ApiSuccessResult<bool>(true, "Success");
        }

        public async Task<ApiResult<bool>> DeletePlatform(DeletePlatformRequest request)
        {
            var platform = await _context.Platforms.FindAsync(request.PlatformId);
            if (platform == null)
            {
                return new ApiErrorResult<bool>("Không tìm thấy nền tảng");
            }

            _context.Platforms.Remove(platform);
            await _context.SaveChangesAsync();
            return new ApiSuccessResult<bool>(false, "Success");
        }

        public async Task<ApiResult<PlatfromVm>> GetPlatfromById(int PlatformId)
        {
            var platform = await _context.Platforms.FindAsync(PlatformId);
            if (platform == null)
            {
                return new ApiErrorResult<PlatfromVm>("Không tìm thấy nền tảng");
            }
            var platformVm = new PlatfromVm()
            {
                PlatformLogo = platform.PlatformLogo,
                PlatformUrl = platform.PlatformUrl,
                PlatformName = platform.PlatformName,
                PlatformId = platform.PlatformId,
                Status = platform.Status,
            };
            return new ApiSuccessResult<PlatfromVm>(platformVm, "Success");
        }

        public async Task<ApiResult<bool>> UpdatePlatform(UpdatePlatformRequest request)
        {
            var errorList = new List<string>();
            if (string.IsNullOrEmpty(request.PlatformName))
            {
                errorList.Add("Vui lòng nhập tên nền tảng");
                //return new ApiErrorResult<bool>("Vui lòng nhập tên nền tảng");
            }
            if (string.IsNullOrEmpty(request.PlatformUrl))
            {
                errorList.Add("Vui lòng nhập Url của nền tảng");
                //return new ApiErrorResult<bool>("Vui lòng nhập Url của nền tảng");
            }
            if (request.PlatformLogo == null)
            {
                errorList.Add("Vui lòng gắn Logo");
                //return new ApiErrorResult<bool>("Vui lòng gắn Logo");
            }
            if (errorList.Any())
            {
                return new ApiErrorResult<bool>("Không hợp lệ", errorList);
            }
            var platform = await _context.Platforms.FindAsync(request.PlatformId);
            if (platform == null)
            {
                return new ApiErrorResult<bool>("Không tìm thấy nền tảng");
            }
            string firebaseUrl = await DiamondLuxurySolution.Utilities.Helper.ImageHelper.Upload(request.PlatformLogo);
            platform.PlatformName = request.PlatformName;
            platform.PlatformUrl = request.PlatformUrl;
            platform.PlatformLogo = firebaseUrl;
            platform.Status = request.Status;
            await _context.SaveChangesAsync();
            return new ApiSuccessResult<bool>(true, "Success");
        }

        public async Task<ApiResult<PageResult<PlatfromVm>>> ViewPlatfromInCustomer(ViewPlatformRequest request)
        {
            var listPlatform = await _context.Platforms.ToListAsync();
            if (request.Keyword != null)
            {
                listPlatform = listPlatform.Where(x => x.PlatformName.Contains(request.Keyword)).ToList();

            }
            listPlatform = listPlatform.Where(x => x.Status).OrderByDescending(x => x.PlatformName).ToList();

            int pageIndex = request.pageIndex ?? 1;

            var listPaging = listPlatform.ToPagedList(pageIndex, DiamondLuxurySolution.Utilities.Constants.Systemconstant.AppSettings.PAGE_SIZE).ToList();

            var listPlatformVm = listPaging.Select(x => new PlatfromVm()
            {
                PlatformId = x.PlatformId,
                PlatformName = x.PlatformName,
                PlatformUrl = x.PlatformUrl,
                PlatformLogo = x.PlatformLogo,
                Status = x.Status,
            }).ToList();
            var listResult = new PageResult<PlatfromVm>()
            {
                Items = listPlatformVm,
                PageSize = DiamondLuxurySolution.Utilities.Constants.Systemconstant.AppSettings.PAGE_SIZE,
                TotalRecords = listPlatform.Count,
                PageIndex = pageIndex
            };
            return new ApiSuccessResult<PageResult<PlatfromVm>>(listResult, "Success");
        }

        public async Task<ApiResult<PageResult<PlatfromVm>>> ViewPlatfromInManager(ViewPlatformRequest request)
        {
            var listPlatform = await _context.Platforms.ToListAsync();
            if (request.Keyword != null)
            {
                listPlatform = listPlatform.Where(x => x.PlatformName.Contains(request.Keyword)).ToList();

            }
            listPlatform = listPlatform.OrderByDescending(x => x.PlatformName).ToList();

            int pageIndex = request.pageIndex ?? 1;

            var listPaging = listPlatform.ToPagedList(pageIndex, DiamondLuxurySolution.Utilities.Constants.Systemconstant.AppSettings.PAGE_SIZE).ToList();

            var listPlatformVm = listPaging.Select(x => new PlatfromVm()
            {
                PlatformId = x.PlatformId,
                PlatformName = x.PlatformName,
                PlatformUrl = x.PlatformUrl,
                PlatformLogo = x.PlatformLogo,
                Status = x.Status,
            }).ToList();
            var listResult = new PageResult<PlatfromVm>()
            {
                Items = listPlatformVm,
                PageSize = DiamondLuxurySolution.Utilities.Constants.Systemconstant.AppSettings.PAGE_SIZE,
                TotalRecords = listPlatform.Count,
                PageIndex = pageIndex
            };
            return new ApiSuccessResult<PageResult<PlatfromVm>>(listResult, "Success");
        }
    }
}
