﻿using Azure.Core;
using DiamondLuxurySolution.Data.EF;
using DiamondLuxurySolution.Data.Entities;
using DiamondLuxurySolution.ViewModel.Common;
using DiamondLuxurySolution.ViewModel.Models.Contact;
using DiamondLuxurySolution.ViewModel.Models.Gem;
using DiamondLuxurySolution.ViewModel.Models.GemPriceList;
using DiamondLuxurySolution.ViewModel.Models.InspectionCertificate;
using DiamondLuxurySolution.ViewModel.Models.Promotion;
using Microsoft.EntityFrameworkCore;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondLuxurySolution.Application.Repository.Gem
{
    public class GemRepo : IGemRepo
    {
        private readonly LuxuryDiamondShopContext _context;
        public GemRepo(LuxuryDiamondShopContext context)
        {
            _context = context;
        }
        public async Task<ApiResult<bool>> CreateGem(CreateGemRequest request)
        {
            var insp = await _context.InspectionCertificates.FindAsync(request.InspectionCertificateId);
            if (insp == null)
            {
                return new ApiErrorResult<bool>("Không tìm thấy giấy chứng nhận kim cương");
            }

            var gpl = await _context.GemPriceLists.FindAsync(request.GemPriceListId);
            if (gpl == null)
            {
				return new ApiErrorResult<bool>("Không tìm mã giá kim cương");
			}

            var list = await _context.Gems.ToListAsync();
            foreach (var gems in list)
            {
                if (gems.InspectionCertificateId.Equals(request.InspectionCertificateId))
                {
                    return new ApiErrorResult<bool>("Giấy chứng nhận này đã được sử dụng");
                }
            }

            if (string.IsNullOrEmpty(request.GemName))
            {
                return new ApiErrorResult<bool>("Vui lòng nhập tên kim cương");
            }

            var gem = new DiamondLuxurySolution.Data.Entities.Gem
            {
                GemId = Guid.NewGuid(),
                GemName = request.GemName,
                Polish = request.Polish != null ? request.Polish : "",
                Symetry = request.Symetry != null ? request.Symetry : "",
                IsOrigin = request.IsOrigin,
                Fluoresence = request.Fluoresence,
                AcquisitionDate = (DateTime)request.AcquisitionDate,
                Active = request.Active,
                InspectionCertificateId = request.InspectionCertificateId,
                GemPriceList = gpl,
            };
            if (request.ProportionImage != null)
            {
                string firebaseUrl = await DiamondLuxurySolution.Utilities.Helper.ImageHelper.Upload(request.ProportionImage);
                gem.ProportionImage = firebaseUrl;
            } else
            {
                gem.ProportionImage = "";
            }
            if (request.GemImage != null)
            {
                string firebaseUrl = await DiamondLuxurySolution.Utilities.Helper.ImageHelper.Upload(request.GemImage);
                gem.GemImage = firebaseUrl;
            }
            else
            {
                gem.GemImage = "";
            }

            _context.Gems.Add(gem);
            await _context.SaveChangesAsync();
            return new ApiSuccessResult<bool>(true, "Success");
        }

        public async Task<ApiResult<bool>> DeleteGem(DeleteGemRequest request)
        {
            var gem = await _context.Gems.FindAsync(request.GemId);
            if (gem == null)
            {
                return new ApiErrorResult<bool>("Không tìm thấy kim cương");
            }

            _context.Gems.Remove(gem);
            await _context.SaveChangesAsync();
            return new ApiSuccessResult<bool>(false, "Success");
        }

        public async Task<ApiResult<List<GemVm>>> GetAll()
        {
            var list = await _context.Gems.ToListAsync();
            var listGemVm = new List<GemVm>();

            foreach (var item in list)
            {
                var insp = await _context.InspectionCertificates.FindAsync(item.InspectionCertificateId);
                var inspectionCertificateVm = new InspectionCertificateVm()
                {
                    InspectionCertificateId = insp.InspectionCertificateId,
                    InspectionCertificateName = insp.InspectionCertificateName,
                    DateGrading = insp.DateGrading,
                    Logo = insp.Logo,
                    Status = insp.Status,
                };
                var gpl = await _context.GemPriceLists.FindAsync(item.GemPriceListId);
                var gplVm = new GemPriceListVm()
                {
                    GemPriceListId = gpl.GemPriceListId,
                    CaratWeight = gpl.CaratWeight,
                    Active = gpl.Active,
                    Clarity = gpl.Clarity,
                    Color = gpl.Color,
                    Cut = gpl.Cut,
                    effectDate = gpl.effectDate,
                    Price = (decimal)gpl.Price,
                };
                var gemVm = new GemVm()
                {
                    GemId = item.GemId,
                    GemName = item.GemName,
                    Polish = item.Polish,
                    Symetry = item.Symetry,
                    IsOrigin = item.IsOrigin,
                    GemImage = item.GemImage,
                    Fluoresence = item.Fluoresence,
                    ProportionImage = item.ProportionImage,
                    AcquisitionDate = item.AcquisitionDate,
                    Active = item.Active,
                    InspectionCertificateVm = inspectionCertificateVm,
                    GemPriceListVm = gplVm,
                };
                listGemVm.Add(gemVm);
            }
            return new ApiSuccessResult<List<GemVm>>(listGemVm.ToList());
        }

        public async Task<ApiResult<GemVm>> GetGemById(Guid GemId)
        {
            var gem = await _context.Gems.FindAsync(GemId);
            if (gem == null)
            {
                return new ApiErrorResult<GemVm>("Không tìm thấy kim cương");
            }
            var insp = await _context.InspectionCertificates.FindAsync(gem.InspectionCertificateId);
            var inspectionCertificateVm = new InspectionCertificateVm() 
            {
                InspectionCertificateId = insp.InspectionCertificateId,
                InspectionCertificateName = insp.InspectionCertificateName,
                DateGrading = insp.DateGrading,
                Logo = insp.Logo,
                Status = insp.Status,          
            };
			var gpl = await _context.GemPriceLists.FindAsync(gem.GemPriceListId);
			var gplVm = new GemPriceListVm()
			{
				GemPriceListId = gpl.GemPriceListId,
				CaratWeight = gpl.CaratWeight,
				Active = gpl.Active,
				Clarity = gpl.Clarity,
				Color = gpl.Color,
				Cut = gpl.Cut,
				effectDate = gpl.effectDate,
				Price = (decimal)gpl.Price,
			};

			var gemVm = new GemVm()
            {
                GemId = gem.GemId,
                GemName = gem.GemName,
                Polish = gem.Polish,
                Symetry = gem.Symetry,
                IsOrigin = gem.IsOrigin,
                GemImage = gem.GemImage,
                Fluoresence = gem.Fluoresence,
                ProportionImage = gem.ProportionImage,
                AcquisitionDate = gem.AcquisitionDate,
                Active = gem.Active,
                InspectionCertificateVm = inspectionCertificateVm,
                GemPriceListVm = gplVm,
            };
            return new ApiSuccessResult<GemVm>(gemVm, "Success");
        }

        public async Task<ApiResult<bool>> UpdateGem(UpdateGemResquest request)
        {
            if (string.IsNullOrEmpty(request.GemName))
            {
                return new ApiErrorResult<bool>("Vui lòng nhập tên kim cương");
            }
            var gem = await _context.Gems.FindAsync(request.GemId);
            if (gem == null)
            {
                return new ApiErrorResult<bool>("Không tìm thấy kim cương");
            }

			var gpl = await _context.GemPriceLists.FindAsync(request.GemPriceListId);
			if (gpl == null)
			{
				return new ApiErrorResult<bool>("Không tìm mã giá kim cương");
			}

			gem.GemName = request.GemName;
            gem.Polish = request.Polish != null ? request.Polish : "";
            gem.Symetry = request.Symetry != null ? request.Symetry : "";
            gem.IsOrigin = request.IsOrigin;
            gem.Fluoresence = request.Fluoresence;
            gem.AcquisitionDate = (DateTime)request.AcquisitionDate;
            gem.Active = request.Active;
            gem.GemPriceList = gpl;
            if (request.ProportionImage != null)
            {
                string firebaseUrl = await DiamondLuxurySolution.Utilities.Helper.ImageHelper.Upload(request.ProportionImage);
                gem.ProportionImage = firebaseUrl;
            } 
            if (request.GemImage != null)
            {
                string firebaseUrl = await DiamondLuxurySolution.Utilities.Helper.ImageHelper.Upload(request.GemImage);
                gem.GemImage = firebaseUrl;
            }
            

            await _context.SaveChangesAsync();
            return new ApiSuccessResult<bool>(true, "Success");
        }

        public async Task<ApiResult<PageResult<GemVm>>> ViewGemInCustomer(ViewGemRequest request)
        {
            var listGem = await _context.Gems.ToListAsync();
            if (request.Keyword != null)
            {
                listGem = listGem.Where(x => x.GemName.Contains(request.Keyword, StringComparison.OrdinalIgnoreCase)).ToList();

            }
            listGem = listGem.Where(x => x.Active is true).OrderByDescending(x => x.GemName).ToList();

            int pageIndex = request.pageIndex ?? 1;

            var listPaging = listGem.ToPagedList(pageIndex, DiamondLuxurySolution.Utilities.Constants.Systemconstant.AppSettings.PAGE_SIZE).ToList();

            var listGemVm = new List<GemVm>();

            foreach (var item in listPaging)
            {
                var insp = await _context.InspectionCertificates.FindAsync(item.InspectionCertificateId);
                var inspectionCertificateVm = new InspectionCertificateVm()
                {
                    InspectionCertificateId = insp.InspectionCertificateId,
                    InspectionCertificateName = insp.InspectionCertificateName,
                    DateGrading = insp.DateGrading,
                    Logo = insp.Logo,
                    Status = insp.Status,
                };
				var gpl = await _context.GemPriceLists.FindAsync(item.GemPriceListId);
				var gplVm = new GemPriceListVm()
				{
					GemPriceListId = gpl.GemPriceListId,
					CaratWeight = gpl.CaratWeight,
					Active = gpl.Active,
					Clarity = gpl.Clarity,
					Color = gpl.Color,
					Cut = gpl.Cut,
					effectDate = gpl.effectDate,
					Price = (decimal)gpl.Price,
				};
				var gemVm = new GemVm()
                {
                    GemId = item.GemId,
                    GemName = item.GemName,
                    Polish = item.Polish,
                    Symetry = item.Symetry,
                    IsOrigin = item.IsOrigin,
                    GemImage = item.GemImage,
                    Fluoresence = item.Fluoresence,
                    ProportionImage = item.ProportionImage,
                    AcquisitionDate = item.AcquisitionDate,
                    Active = item.Active,
                    InspectionCertificateVm = inspectionCertificateVm,
                    GemPriceListVm = gplVm,
                };
                listGemVm.Add(gemVm);
            }

            var listResult = new PageResult<GemVm>()
            {
                Items = listGemVm,
                PageSize = DiamondLuxurySolution.Utilities.Constants.Systemconstant.AppSettings.PAGE_SIZE,
                TotalRecords = listGem.Count,
                PageIndex = pageIndex
            };
            return new ApiSuccessResult<PageResult<GemVm>>(listResult, "Success");
        }

        public async Task<ApiResult<PageResult<GemVm>>> ViewGemInManager(ViewGemRequest request)
        {
            var listGem = await _context.Gems.ToListAsync();
            if (request.Keyword != null)
            {
                listGem = listGem.Where(x => x.GemName.Contains(request.Keyword, StringComparison.OrdinalIgnoreCase)).ToList();

            }
            listGem = listGem.OrderByDescending(x => x.GemName).ToList();

            int pageIndex = request.pageIndex ?? 1;

            var listPaging = listGem.ToPagedList(pageIndex, DiamondLuxurySolution.Utilities.Constants.Systemconstant.AppSettings.PAGE_SIZE).ToList();

            var listGemVm = new List<GemVm>();

            foreach (var item in listPaging)
            {
                var insp = await _context.InspectionCertificates.FindAsync(item.InspectionCertificateId);
                var inspectionCertificateVm = new InspectionCertificateVm()
                {
                    InspectionCertificateId = insp.InspectionCertificateId,
                    InspectionCertificateName = insp.InspectionCertificateName,
                    DateGrading = insp.DateGrading,
                    Logo = insp.Logo,
                    Status = insp.Status,
                };
				var gpl = await _context.GemPriceLists.FindAsync(item.GemPriceListId);
				var gplVm = new GemPriceListVm()
				{
                    GemPriceListId = gpl.GemPriceListId,
					CaratWeight = gpl.CaratWeight,
					Active = gpl.Active,
					Clarity = gpl.Clarity,
					Color = gpl.Color,
					Cut = gpl.Cut,
					effectDate = gpl.effectDate,
					Price = (decimal)gpl.Price,
				};
				var gemVm = new GemVm()
                {
                    GemId = item.GemId,
                    GemName = item.GemName,
                    Polish = item.Polish,
                    Symetry = item.Symetry,
                    IsOrigin = item.IsOrigin,
                    GemImage = item.GemImage,
                    Fluoresence = item.Fluoresence,
                    ProportionImage = item.ProportionImage,
                    AcquisitionDate = item.AcquisitionDate,
                    Active = item.Active,
                    InspectionCertificateVm = inspectionCertificateVm,
                    GemPriceListVm = gplVm
                };
                listGemVm.Add(gemVm);
            }

            var listResult = new PageResult<GemVm>()
            {
                Items = listGemVm,
                PageSize = DiamondLuxurySolution.Utilities.Constants.Systemconstant.AppSettings.PAGE_SIZE,
                TotalRecords = listGem.Count,
                PageIndex = pageIndex
            };
            return new ApiSuccessResult<PageResult<GemVm>>(listResult, "Success");
        }
    }
}
