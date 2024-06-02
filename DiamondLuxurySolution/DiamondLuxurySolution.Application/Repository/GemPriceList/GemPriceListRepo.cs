﻿using DiamondLuxurySolution.Data.EF;
using DiamondLuxurySolution.Data.Entities;
using DiamondLuxurySolution.ViewModel.Common;
using DiamondLuxurySolution.ViewModel.Models.Gem;
using DiamondLuxurySolution.ViewModel.Models.GemPriceList;
using DiamondLuxurySolution.ViewModel.Models.InspectionCertificate;
using DiamondLuxurySolution.ViewModel.Models.News;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace DiamondLuxurySolution.Application.Repository.GemPriceList
{
    public class GemPriceListRepo : IGemPriceListRepo
    {
        private readonly LuxuryDiamondShopContext _context;

        public GemPriceListRepo(LuxuryDiamondShopContext context)
        {
            _context = context;
        }
        public async Task<ApiResult<bool>> CreateGemPriceList(CreateGemPriceListRequest request)
        {
            var gem = await _context.Gems.FindAsync(request.GemId);
            if (gem == null)
            {
                return new ApiErrorResult<bool>("Không tìm thấy kim cương");
            }

            var errorList = new List<string>();
            if (string.IsNullOrEmpty(request.Price))
            {
                errorList.Add("Vui lòng nhập giá kim cương");
            }

            double price = 0;
            try
            {
                price = Convert.ToDouble(request.Price);

                if (price <= 0)
                {
                    errorList.Add("Giá kim cương > 0");
                }
            }
            catch (FormatException)
            {
                errorList.Add("Giá kim cương không hợp lệ");
            }

            if (request.effectDate < DateTime.Today.AddDays(-7) || request.effectDate > DateTime.Today)
            {
                errorList.Add("Bảng giá kim cương phải được cập nhật trong khoảng thời gian gần đây.");
            }


            if (errorList.Any())
            {
                return new ApiErrorResult<bool>("Không hợp lệ", errorList);
            }

            var gemPriceList = new Data.Entities.GemPriceList
            {
                CaratWeight = !string.IsNullOrWhiteSpace(request.CaratWeight) ? request.CaratWeight : "",
                Clarity = request.Clarity != null ? request.Clarity : "",
                Color = request.Color != null ? request.Color : "",
                Price = price,
                Cut = request.Cut != null ? request.Cut : "",
                GemId = request.GemId,
                Active = request.Active,
                effectDate = (DateTime)request.effectDate
            };

            _context.GemPriceLists.Add(gemPriceList);
            await _context.SaveChangesAsync();
            return new ApiSuccessResult<bool>(true, "Success");
        }

        public async Task<ApiResult<bool>> DeleteGemPriceList(DeleteGemPriceListRequest request)
        {
            var gemPriceList = await _context.GemPriceLists.FindAsync(request.GemPriceListId);
            if (gemPriceList == null)
            {
                return new ApiErrorResult<bool>("Không tìm thấy bảng giá kim cương");
            }
            _context.GemPriceLists.Remove(gemPriceList);
            await _context.SaveChangesAsync();
            return new ApiSuccessResult<bool>(false, "Success");
        }

        public async Task<ApiResult<List<GemPriceListVm>>> GetAll()
        {
            var list = await _context.GemPriceLists.ToListAsync();
            var listGemPriceListVm = new List<GemPriceListVm>();

            foreach (var item in list)
            {
                var gem = await _context.Gems.FindAsync(item.GemId);
                var insp = await _context.InspectionCertificates.FindAsync(gem.InspectionCertificateId);
                var inspectionCertificateVm = new InspectionCertificateVm()
                {
                    InspectionCertificateId = insp.InspectionCertificateId,
                    InspectionCertificateName = insp.InspectionCertificateName,
                    DateGrading = insp.DateGrading,
                    Logo = insp.Logo,
                    Status = insp.Status,
                };
                var gemVm = new GemVm()
                {
                    GemId = gem.GemId,
                    GemName = gem.GemName,
                    AcquisitionDate = gem.AcquisitionDate,
                    Active = gem.Active,
                    Fluoresence = gem.Fluoresence,
                    GemImage = gem.GemImage,
                    IsOrigin = gem.IsOrigin,
                    Polish = gem.Polish,
                    ProportionImage = gem.ProportionImage,
                    Symetry = gem.Symetry,
                    InspectionCertificateVm = inspectionCertificateVm,
                };
                var gemPriceListVm = new GemPriceListVm
                {
                    GemPriceListId = item.GemPriceListId,
                    Active = item.Active,
                    CaratWeight = item.CaratWeight,
                    Clarity = item.Clarity,
                    Color = item.Color,
                    Cut = item.Cut,
                    effectDate = item.effectDate,
                    Price = (double)item.Price,
                    GemVm = gemVm
                };
                listGemPriceListVm.Add(gemPriceListVm);
            }
            return new ApiSuccessResult<List<GemPriceListVm>>(listGemPriceListVm.ToList());
        }

        public async Task<ApiResult<GemPriceListVm>> GetGemPriceListById(int GemPriceListId)
        {
            var GemPriceList = await _context.GemPriceLists.FindAsync(GemPriceListId);
            if (GemPriceList == null)
            {
                return new ApiErrorResult<GemPriceListVm>("Không tìm thấy bảng giá kim cương");
            }
            var gem = await _context.Gems.FindAsync(GemPriceList.GemId);
            var insp = await _context.InspectionCertificates.FindAsync(gem.InspectionCertificateId);
            var inspectionCertificateVm = new InspectionCertificateVm()
            {
                InspectionCertificateId = insp.InspectionCertificateId,
                InspectionCertificateName = insp.InspectionCertificateName,
                DateGrading = insp.DateGrading,
                Logo = insp.Logo,
                Status = insp.Status,
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
            };
            var gemPriceListVm = new GemPriceListVm
            {
                GemPriceListId = GemPriceListId,
                CaratWeight = GemPriceList.CaratWeight,
                Clarity = GemPriceList.Clarity,
                Color = GemPriceList.Color,
                Cut = GemPriceList.Cut,
                Price = (double)GemPriceList.Price,
                effectDate = GemPriceList.effectDate,
                Active = GemPriceList.Active,
                GemVm = gemVm
            };
            return new ApiSuccessResult<GemPriceListVm>(gemPriceListVm, "Success");
        }

        public async Task<ApiResult<bool>> UpdateGemPriceList(UpdateGemPriceListRequest request)
        {
            var GemPriceList = await _context.GemPriceLists.FindAsync(request.GemPriceListId);
            if (GemPriceList == null)
            {
                return new ApiErrorResult<bool>("Không tìm thấy bảng giá kim cương");
            }

            var gem = await _context.Gems.FindAsync(request.GemId);
            if (gem == null)
            {
                return new ApiErrorResult<bool>("Không tìm thấy kim cương");
            }

            var errorList = new List<string>();
            if (string.IsNullOrEmpty(request.Price))
            {
                errorList.Add("Vui lòng nhập giá kim cương");
            }

            double price = 0;
            try
            {
                price = Convert.ToDouble(request.Price);

                if (price <= 0)
                {
                    errorList.Add("Giá kim cương > 0");
                }
            }
            catch (FormatException)
            {
                errorList.Add("Giá kim cương không hợp lệ");
            }

            if (request.effectDate < DateTime.Today.AddDays(-7) || request.effectDate > DateTime.Today)
            {
                errorList.Add("Bảng giá kim cương phải được cập nhật trong khoảng thời gian gần đây.");
            }


            if (errorList.Any())
            {
                return new ApiErrorResult<bool>("Không hợp lệ", errorList);
            }
            GemPriceList.CaratWeight = request.CaratWeight != null ? request.CaratWeight : "";
            GemPriceList.Cut = request.Cut != null ? request.Cut : "";
            GemPriceList.Clarity = request.Clarity != null ? request.Clarity : "";
            GemPriceList.Color = request.Color != null ? request.Color : "";
            GemPriceList.Price = price;
            GemPriceList.effectDate = (DateTime)request.effectDate;
            GemPriceList.Active = request.Active;
            await _context.SaveChangesAsync();
            return new ApiSuccessResult<bool>(true, "Success");
        }

        public async Task<ApiResult<PageResult<GemPriceListVm>>> ViewGemPriceList(ViewGemPriceListRequest request)
        {
            var listGemPriceList = await _context.GemPriceLists.ToListAsync();
            if (request.Keyword != null)
            {
                listGemPriceList = listGemPriceList.Where(x => x.CaratWeight.Contains(request.Keyword)).ToList();

            }
            listGemPriceList = listGemPriceList.Where(x => x.Active == true).OrderByDescending(x => x.CaratWeight).ToList();

            int pageIndex = request.pageIndex ?? 1;

            var listPaging = listGemPriceList.ToPagedList(pageIndex, DiamondLuxurySolution.Utilities.Constants.Systemconstant.AppSettings.PAGE_SIZE).ToList();
            var listGemPriceListVm = new List<GemPriceListVm>();
            foreach (var item in listPaging)
            {
                var gem = await _context.Gems.FindAsync(item.GemId);
                var insp = await _context.InspectionCertificates.FindAsync(gem.InspectionCertificateId);
                var inspectionCertificateVm = new InspectionCertificateVm()
                {
                    InspectionCertificateId = insp.InspectionCertificateId,
                    InspectionCertificateName = insp.InspectionCertificateName,
                    DateGrading = insp.DateGrading,
                    Logo = insp.Logo,
                    Status = insp.Status,
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
                };
                var gemPriceListVm = new GemPriceListVm()
                {
                    GemPriceListId = item.GemPriceListId,
                    CaratWeight = item.CaratWeight,
                    Clarity = item.Clarity,
                    Cut = item.Cut,
                    Color = item.Color,
                    Price = (double)item.Price,
                    effectDate = item.effectDate,
                    Active = item.Active,
                    GemVm = gemVm
                };
                listGemPriceListVm.Add(gemPriceListVm);
            }
            var listResult = new PageResult<GemPriceListVm>()
            {
                Items = listGemPriceListVm,
                PageSize = DiamondLuxurySolution.Utilities.Constants.Systemconstant.AppSettings.PAGE_SIZE,
                TotalRecords = listGemPriceList.Count,
                PageIndex = pageIndex
            };
            return new ApiSuccessResult<PageResult<GemPriceListVm>>(listResult, "Success");
        }
    }
}
