﻿using DiamondLuxurySolution.AdminCrewApp.Service.Promotion;
using DiamondLuxurySolution.Data.EF;
using DiamondLuxurySolution.Data.Entities;
using DiamondLuxurySolution.ViewModel.Common;
using DiamondLuxurySolution.ViewModel.Models.Promotion;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace DiamondLuxurySolution.AdminCrewApp.Controllers
{
    [Authorize(Roles = DiamondLuxurySolution.Utilities.Constants.Systemconstant.UserRoleDefault.Admin + ", " + DiamondLuxurySolution.Utilities.Constants.Systemconstant.UserRoleDefault.Manager)]

    public class PromotionController : BaseController
    {
        private readonly IPromotionApiService _promotionApiService;

        public PromotionController(IPromotionApiService promotionApiService)
        {
            _promotionApiService = promotionApiService;
        }

        [HttpGet]
        public async Task<IActionResult> Index(ViewPromotionRequest request)
        {
            try
            {

                ViewBag.txtLastSeachValue = request.Keyword;
                if (!ModelState.IsValid)
                {
                    return View();
                }
                if (TempData["FailMsg"] != null)
                {
                    ViewBag.FailMsg = TempData["FailMsg"];
                }
                if (TempData["SuccessMsg"] != null)
                {
                    ViewBag.SuccessMsg = TempData["SuccessMsg"];
                }

                var Promotion = await _promotionApiService.ViewPromotionInManager(request);
                return View(Promotion.ResultObj);
            }
            catch
            {
                return View();
            }
        }
        [HttpGet]
        public async Task<IActionResult> Detail(Guid PromotionId)
        {
            try
            {
                var status = await _promotionApiService.GetPromotionById(PromotionId);
                if (status is ApiErrorResult<PromotionVm> errorResult)
                {
                    List<string> listError = new List<string>();
                    if (status.Message != null)
                    {
                        listError.Add(errorResult.Message);
                    }
                    else if (errorResult.ValidationErrors != null && errorResult.ValidationErrors.Count > 0)
                    {
                        foreach (var error in listError)
                        {
                            listError.Add(error);
                        }
                    }
                    ViewBag.Errors = listError;
                    return View();

                }
                return View(status.ResultObj);
            }
            catch
            {
                return View();
            }
        }

        [HttpGet]
        public async Task<IActionResult> Edit(Guid PromotionId)
        {
            try
            {
                var Promotion = await _promotionApiService.GetPromotionById(PromotionId);
                if (Promotion is ApiErrorResult<PromotionVm> errorResult)
                {
                    List<string> listError = new List<string>();
                    if (Promotion.Message != null)
                    {
                        listError.Add(errorResult.Message);
                    }
                    else if (errorResult.ValidationErrors != null && errorResult.ValidationErrors.Count > 0)
                    {
                        foreach (var error in listError)
                        {
                            listError.Add(error);
                        }
                    }
                    ViewBag.Errors = listError;
                    return View();

                }

                // Chuyển đổi Price từ decimal sang string để xử lý
                string priceString = Promotion.ResultObj.MaxDiscount.ToString();

                // Cắt bỏ hai số 0 cuối cùng nếu chúng tồn tại
                if (priceString.EndsWith("00"))
                {
                    priceString = priceString.Substring(0, priceString.Length - 2);
                }

                // Gán lại giá trị đã xử lý cho Price (nếu cần thiết)
                Promotion.ResultObj.MaxDiscount = decimal.Parse(priceString);

                return View(Promotion.ResultObj);
            }
            catch
            {
                return View();
            }
        }
        [HttpPost]
        public async Task<IActionResult> Edit(UpdatePromotionRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var promotionsVmCall = await _promotionApiService.GetPromotionById(request.PromotionId);


                    PromotionVm promotionVm = new PromotionVm()
                    {
                        PromotionId = request.PromotionId,
                        PromotionName = request.PromotionName,
                        Description = request.Description,
                        PromotionImage = promotionsVmCall.ResultObj.PromotionImage,
                        StartDate = (DateTime)request.StartDate,
                        EndDate = (DateTime)request.EndDate,
                        BannerImage = promotionsVmCall.ResultObj.BannerImage,
                        DiscountPercent = Convert.ToDecimal(request.DiscountPercent),
                        MaxDiscount = Convert.ToDecimal(request.MaxDiscount),
                        Status = request.Status,
                    };

                    return View(promotionVm);
                }

                var status = await _promotionApiService.UpdatePromotion(request);
                if (status is ApiErrorResult<bool> errorResult)
                {
                    List<string> listError = new List<string>();
                    if (status.Message != null)
                    {
                        listError.Add(errorResult.Message);
                    }
                    else if (errorResult.ValidationErrors != null && errorResult.ValidationErrors.Count > 0)
                    {
                        foreach (var error in listError)
                        {
                            listError.Add(error);
                        }
                    }
                    ViewBag.Errors = listError;
                    return View();

                }
                return RedirectToAction("Index", "Promotion");
            }
            catch
            {
                return View();
            }
        }



        [HttpGet]
        public async Task<IActionResult> Delete(Guid PromotionId)
        {
            try
            {
                var Promotion = await _promotionApiService.GetPromotionById(PromotionId);
                if (Promotion is ApiErrorResult<PromotionVm> errorResult)
                {
                    List<string> listError = new List<string>();
                    if (Promotion.Message != null)
                    {
                        listError.Add(errorResult.Message);
                    }
                    else if (errorResult.ValidationErrors != null && errorResult.ValidationErrors.Count > 0)
                    {
                        foreach (var error in listError)
                        {
                            listError.Add(error);
                        }
                    }
                    ViewBag.Errors = listError;
                    return View();

                }
                return View(Promotion.ResultObj);
            }
            catch
            {
                return View();
            }
        }

        [HttpPost]
        public async Task<IActionResult> Delete(DeletePromotionRequest request)
        {
            try
            {
                var status = await _promotionApiService.DeletePromotion(request);
                if (status is ApiErrorResult<bool> errorResult)
                {
                    List<string> listError = new List<string>();
                    if (status.Message != null)
                    {
                        listError.Add(errorResult.Message);
                    }
                    else if (errorResult.ValidationErrors != null && errorResult.ValidationErrors.Count > 0)
                    {
                        foreach (var error in listError)
                        {
                            listError.Add(error);
                        }
                    }
                    ViewBag.Errors = listError;
                    return View();

                }
                return RedirectToAction("Index", "Promotion");
            }
            catch
            {
                return View();
            }
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {

            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(CreatePromotionRequest request)
        {
            if (!ModelState.IsValid)
            {
                return View(request);
            }

            var status = await _promotionApiService.CreatePromotion(request);

            if (status is ApiErrorResult<bool> errorResult)
            {
                List<string> listError = new List<string>();

                if (errorResult.ValidationErrors != null && errorResult.ValidationErrors.Count > 0)
                {
                    foreach (var error in errorResult.ValidationErrors)
                    {
                        listError.Add(error);
                    }
                }
                else if (status.Message != null)
                {
                    listError.Add(errorResult.Message);
                }
                ViewBag.Errors = listError;
                return View();

            }
            TempData["SuccessMsg"] = "Tạo mới thành công cho " + request.PromotionName;

            return RedirectToAction("Index", "Promotion");
        }
    }
}
