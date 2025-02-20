﻿using DiamondLuxurySolution.AdminCrewApp.Service.IInspectionCertificate;
using DiamondLuxurySolution.AdminCrewApp.Service.SubGem;
using DiamondLuxurySolution.Data.Entities;
using DiamondLuxurySolution.ViewModel.Common;
using DiamondLuxurySolution.ViewModel.Models.GemPriceList;
using DiamondLuxurySolution.ViewModel.Models.InspectionCertificate;
using DiamondLuxurySolution.ViewModel.Models.SubGem;
using Microsoft.AspNetCore.Mvc;

namespace DiamondLuxurySolution.AdminCrewApp.Controllers
{
    public class SubGemController : Controller
    {
        private readonly ISubGemApiService _isubGemApiService;
        public SubGemController(ISubGemApiService isubGemApiService)
        {
            _isubGemApiService = isubGemApiService;
        }

        [HttpGet]
        public async Task<IActionResult> Index(ViewSubGemRequest request)
        {
            try
            {
                ViewBag.txtLastSearchValue = request.KeyWord;
                if(!ModelState.IsValid)
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

                var subgem = await _isubGemApiService.ViewSubGemInManager(request);
                return View(subgem.ResultObj);
            }
            catch (Exception)
            {
                return View();
            }
        }
        [HttpGet]
        public async Task<IActionResult> Detail(Guid SubGemId)
        {
            try
            {
                var status = await _isubGemApiService.GetSubGemId(SubGemId);
                if (status is ApiErrorResult<SubGemVm> errorResult)
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
                    TempData["ErrorToast"] = true;
                    ViewBag.Errors = listError;
                    return View();

                }
                return View(status.ResultObj);
            }
            catch
            {
                TempData["ErrorToast"] = true;
                return View();
            }
        }

        [HttpGet]
        public async Task<IActionResult> Edit(Guid SubGemId)
        {
            try
            {
                var subgem = await _isubGemApiService.GetSubGemId(SubGemId);
                if (subgem is ApiErrorResult<SubGemVm> errorResult)
                {
                    List<string> listError = new List<string>();
                    if (subgem.Message != null)
                    {
                        listError.Add(errorResult.Message);
                    }
                    else if (errorResult.ValidationErrors != null && errorResult.ValidationErrors.Count > 0)
                    {
                        foreach (var error in errorResult.ValidationErrors)
                        {
                            listError.Add(error);
                        }
                    }
                    TempData["ErrorToast"] = true;
                    ViewBag.Errors = listError;
                    return View();
                }
                // Chuyển đổi Price từ decimal sang string để xử lý
                string priceString = subgem.ResultObj.SubGemPrice.ToString("0.##");

                // Gán lại giá trị đã xử lý cho Price (nếu cần thiết)
                subgem.ResultObj.SubGemPrice = decimal.Parse(priceString);

                return View(subgem.ResultObj);
            }
            catch
            {
                TempData["ErrorToast"] = true;
                return View();
            }
        }

        [HttpPost]
        public async Task<IActionResult> Edit(UpdateSubGemRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var subgem = await _isubGemApiService.GetSubGemId(request.SubGemId);

                    SubGemVm subGemVm = new SubGemVm()
                    {
                        SubGemId = request.SubGemId,
                        Active = request.Active,
                        Description = request.Description,
                        SubGemName = request.SubGemName,
                        SubGemPrice = subgem.ResultObj.SubGemPrice,
                    };
                    TempData["WarningToast"] = true;
                    return View(subGemVm);
                }
                var status = await _isubGemApiService.UpdateSubGem(request);
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
                    TempData["WarningToast"] = true;
                    ViewBag.Errors = listError;
                    return View();

                }
                TempData["SuccessToast"] = true;
                return RedirectToAction("Index", "SubGem");
            }
            catch
            {
                TempData["WarningToast"] = true;
                return View();
            }
        }



        [HttpGet]
        public async Task<IActionResult> Delete(Guid SubGemId)
        {
            try
            {

                var subgem = await _isubGemApiService.GetSubGemId(SubGemId);
                if (subgem is ApiErrorResult<SubGemVm> errorResult)
                {
                    List<string> listError = new List<string>();
                    if (subgem.Message != null)
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
                    TempData["ErrorToast"] = true;
                    ViewBag.Errors = listError;
                    return View();

                }
                return View(subgem.ResultObj);
            }
            catch
            {
                TempData["ErrorToast"] = true;
                return View();
            }
        }

        [HttpPost]
        public async Task<IActionResult> Delete(DeleteSubGemRequest request)
        {
            try
            {
                var status = await _isubGemApiService.DeleteSubGem(request);
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
                    TempData["ErrorToast"] = true;
                    ViewBag.Errors = listError;
                    return View();

                }
                TempData["SuccessToast"] = true;
                return RedirectToAction("Index", "SubGem");

            }
            catch
            {
                TempData["ErrorToast"] = true;
                return View();
            }
        }
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(CreateSubGemRequest request)
        {
            var status = await _isubGemApiService.CreateSubGem(request);
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
                TempData["WarningToast"] = true;
                ViewBag.Errors = listError;
                return View();

            }
            TempData["SuccessToast"] = true;

            return RedirectToAction("Index", "SubGem");
        }


    }

}

