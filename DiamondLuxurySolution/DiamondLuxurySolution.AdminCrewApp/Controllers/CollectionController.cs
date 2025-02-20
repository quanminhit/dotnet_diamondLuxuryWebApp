﻿using DiamondLuxurySolution.AdminCrewApp.Service.Collection;
using DiamondLuxurySolution.AdminCrewApp.Service.Product;
using DiamondLuxurySolution.Data.EF;
using DiamondLuxurySolution.Data.Entities;
using DiamondLuxurySolution.ViewModel.Common;
using DiamondLuxurySolution.ViewModel.Models.Collection;
using DiamondLuxurySolution.ViewModel.Models.Product;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NuGet.Packaging;

namespace DiamondLuxurySolution.AdminCrewApp.Controllers
{
    public class CollectionController : Controller
    {
        private readonly ICollectionApiService _collectionApiService;
        private readonly IProductApiService _productApiService;

        public CollectionController(ICollectionApiService collectionApiService,
            IProductApiService productApiService)
        {
            _collectionApiService = collectionApiService;
            _productApiService = productApiService;
        }

        [Authorize(Roles = DiamondLuxurySolution.Utilities.Constants.Systemconstant.UserRoleDefault.Manager)]
        [HttpGet]
        public async Task<IActionResult> IndexProductsCreate()
        {
            try
            {
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

                var Product = await _productApiService.GetAllProduct();
                var listIdSelected = TempData["SaveSelectedIdIndexProductsCreate"] as string;
                if (!string.IsNullOrEmpty(listIdSelected))
                {
                    // Chuyển đổi chuỗi thành danh sách các chuỗi
                    var selectedIdsList = listIdSelected.Split(',').ToList();
                    ViewBag.SelectedIdsIndexProductsCreateHold = selectedIdsList;
                    Product.ResultObj.RemoveAll(p => selectedIdsList.Contains(p.ProductId));
                }
                return View(Product.ResultObj);
            }
            catch
            {
                return View();
            }
        }
        [Authorize(Roles = DiamondLuxurySolution.Utilities.Constants.Systemconstant.UserRoleDefault.Manager)]

        [HttpPost]
        public IActionResult RemoveProduct(string productId)
        {
            var selectedIdsString = HttpContext.Session.GetString("SelectedIds");

            if (!string.IsNullOrEmpty(selectedIdsString))
            {
                var selectedIdsList = selectedIdsString.Split(',').ToList();
                selectedIdsList.Remove(productId);
                HttpContext.Session.SetString("SelectedIds", string.Join(",", selectedIdsList));
            }
			TempData["SuccessToast"] = true;
			return RedirectToAction("Create");
        }
        [Authorize(Roles = DiamondLuxurySolution.Utilities.Constants.Systemconstant.UserRoleDefault.Manager)]

        [HttpPost]
        public async Task<IActionResult> SaveSelectedIdIndexProductsCreate(string listIdSelected)
        {
            try
            {
                TempData["SaveSelectedIdIndexProductsCreate"] = listIdSelected;
                return RedirectToAction("IndexProductsCreate", "Collection");
            }
            catch (Exception)
            {
                return View();
            }
        }
        [Authorize(Roles = DiamondLuxurySolution.Utilities.Constants.Systemconstant.UserRoleDefault.Manager)]

        [HttpPost]
        public async Task<IActionResult> SaveSelectedIdCreate(string selectedIds)
        {
            try
            {
                TempData["SelectedIdsCreate"] = selectedIds;
				TempData["SuccessToast"] = true;
				return RedirectToAction("Create", "Collection");
            }
            catch (Exception)
            {
                return View();
            }
        }
        [Authorize(Roles = DiamondLuxurySolution.Utilities.Constants.Systemconstant.UserRoleDefault.Manager)]

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var selectedIdsString = TempData["SelectedIdsCreate"] as string;

            /*            var selectedIdsString = HttpContext.Request.Cookies["SelectedIds"];
            */
            if (!string.IsNullOrEmpty(selectedIdsString))
            {
                List<ProductVm> listPorducts;
                // Chuyển đổi chuỗi thành danh sách các chuỗi
                var selectedIdsList = selectedIdsString.Split(',').ToList();
                listPorducts = _collectionApiService.GetProductsByListId(selectedIdsList).Result.ResultObj;
                // Đưa danh sách vào ViewBag
                ViewBag.ProductsVm = listPorducts;
            }
            else
            {
                // Nếu chuỗi rỗng hoặc null, tạo một danh sách trống
                ViewBag.ProductsVm = new List<ProductVm>();
            }
            return View();
        }
        [Authorize(Roles = DiamondLuxurySolution.Utilities.Constants.Systemconstant.UserRoleDefault.Manager)]

        [HttpPost]
        public async Task<IActionResult> Create(CreateCollectionRequest request)
        {

            if (request.ListProductId.First() != null)
            {
                string listProductsString = request.ListProductId.First().ToString();
                var listProductsId = listProductsString.Split(",").ToList();
                request.ListProductId = listProductsId;
            }
            var status = await _collectionApiService.CreateCollection(request);

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
            return RedirectToAction("Index", "Collection");
        }
        [Authorize(Roles = DiamondLuxurySolution.Utilities.Constants.Systemconstant.UserRoleDefault.Manager)]

        [HttpPost]
        public async Task<IActionResult> SaveSelectedIdIndexProductsUpdate(string listIdSelected)
        {
            try
            {
                TempData["SelectedIdsIndexProductsUpdate"] = listIdSelected;
				TempData["SuccessToast"] = true;
				return RedirectToAction("IndexProductsUpdate", "Collection");
            }
            catch (Exception)
            {
                return View();
            }
        }
        [Authorize(Roles = DiamondLuxurySolution.Utilities.Constants.Systemconstant.UserRoleDefault.Manager)]

        [HttpGet]
        public async Task<IActionResult> IndexProductsUpdate()
        {
            try
            {
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

                var Product = await _productApiService.GetAllProduct();
                var listIdSelected = TempData["SelectedIdsIndexProductsUpdate"] as string;
                if (!string.IsNullOrEmpty(listIdSelected))
                {
                    // Chuyển đổi chuỗi thành danh sách các chuỗi
                    var selectedIdsList = listIdSelected.Split(',').ToList();
                    ViewBag.SelectedIdsIndexProductsUpdateHold = selectedIdsList;
                    Product.ResultObj.RemoveAll(p => selectedIdsList.Contains(p.ProductId));
                }
                return View(Product.ResultObj);
            }
            catch
            {
                return View();
            }
        }

        [Authorize(Roles = DiamondLuxurySolution.Utilities.Constants.Systemconstant.UserRoleDefault.Manager)]

        [HttpPost]
        public async Task<IActionResult> SaveSelectedIdUpdate(string selectedIds)
        {
            try
            {
                TempData["SelectedIdEdit"] = selectedIds;
				TempData["SuccessToast"] = true;
				return RedirectToAction("Edit", "Collection");
            }
            catch (Exception)
            {
                return View();
            }
        }

        [Authorize(Roles = DiamondLuxurySolution.Utilities.Constants.Systemconstant.UserRoleDefault.Manager)]

        [HttpGet]
        public async Task<IActionResult> Edit(string CollectionId)
        {
            try
            {
                var listProductsAddString = TempData["SelectedIdEdit"] as string;
                List<ProductVm> listProductsAdd = new List<ProductVm>();
                if (!string.IsNullOrEmpty(listProductsAddString))
                {
                    // Chuyển đổi chuỗi thành danh sách các chuỗi
                    var selectedIdsList = listProductsAddString.Split(',').ToList();
                    listProductsAdd = _collectionApiService.GetProductsByListId(selectedIdsList).Result.ResultObj;
                }
                if (CollectionId != null)
                {
                    HttpContext.Session.SetString("CollectionId", CollectionId);
                }
                string collectionIdFinal = !string.IsNullOrEmpty(CollectionId) ? CollectionId : HttpContext.Session.GetString("CollectionId");
                var collection = await _collectionApiService.GetCollectionById(collectionIdFinal);
                if (listProductsAdd.Any())
                {
                    collection.ResultObj.ListProducts = listProductsAdd;
                }
                if (collection is ApiErrorResult<CollectionVm> errorResult)
                {
                    List<string> listError = new List<string>();
                    if (errorResult.ValidationErrors != null && errorResult.ValidationErrors.Count > 0)
                    {
                        foreach (var error in listError)
                        {
                            listError.Add(error);
                        }
                    }
                    else if (collection.Message != null)
                    {
                        listError.Add(errorResult.Message);
                    }
                    TempData["ErrorToast"] = true;
                    ViewBag.Errors = listError;

                    return View();
                }
                collection.ResultObj.priceDisplay = (long)collection.ResultObj.priceDisplay;

                return View(collection.ResultObj);
            }
            catch
            {
                TempData["ErrorToast"] = true;
                return View();
            }
        }
        [Authorize(Roles = DiamondLuxurySolution.Utilities.Constants.Systemconstant.UserRoleDefault.Manager)]

        [HttpPost]
        public async Task<IActionResult> Edit(UpdateCollectionRequest request)
        {
            try
            {
                var collection = await _collectionApiService.GetCollectionById(request.CollectionId);
                HttpContext.Session.Remove("CollectionId");
                if (request.CollectionName == "null")
                {
                    request.CollectionName = null;
                }
                if (!ModelState.IsValid)
                {
                    CollectionVm collectionVm = new CollectionVm()
                    {
                        CollectionId = request.CollectionId,
                        Description = request.Description,
                        Status = request.Status,
                        Thumbnail = collection.ResultObj.Thumbnail,
                    };

                    return View(collectionVm);
                }
                if (request.ListProductsIdDelete.First() != null)
                {
                    string listProductsString = request.ListProductsIdDelete.First().ToString();
                    var listProductsIdDelete = listProductsString.Split(",").ToList();
                    request.ListProductsIdDelete = listProductsIdDelete;
                }
                if (request.ListProductsIdAdd.First() != null)
                {
                    string listProductsString = request.ListProductsIdAdd.First().ToString();
                    var listProductsIdAdd = listProductsString.Split(",").ToList();
                    request.ListProductsIdAdd = listProductsIdAdd;
                }
                var status = await _collectionApiService.UpdateCollection(request);
                if (status is ApiErrorResult<bool> errorResult)
                {
                    List<string> listError = new List<string>();
                    if (errorResult.ValidationErrors != null && errorResult.ValidationErrors.Count > 0)
                    {
                        foreach (var error in listError)
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
                return RedirectToAction("Index", "Collection", new { CollectionId = request.CollectionId });
            }
            catch
            {
                TempData["ErrorToast"] = true;
                return View();
            }
        }

        [Authorize(Roles = DiamondLuxurySolution.Utilities.Constants.Systemconstant.UserRoleDefault.SalesStaff + ", " + DiamondLuxurySolution.Utilities.Constants.Systemconstant.UserRoleDefault.Manager)]
        [HttpGet]
        public async Task<IActionResult> Index(ViewCollectionRequest request)
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

                var collection = await _collectionApiService.ViewCollectionInPaging(request);
                return View(collection.ResultObj);
            }
            catch
            {
                return View();
            }
        }

        [Authorize(Roles = DiamondLuxurySolution.Utilities.Constants.Systemconstant.UserRoleDefault.SalesStaff + ", " + DiamondLuxurySolution.Utilities.Constants.Systemconstant.UserRoleDefault.Manager)]
        [HttpGet]
        public async Task<IActionResult> Detail(string CollectionId)
        {
            try
            {
                var status = await _collectionApiService.GetCollectionById(CollectionId);
                if (status is ApiErrorResult<CollectionVm> errorResult)
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
        [Authorize(Roles = DiamondLuxurySolution.Utilities.Constants.Systemconstant.UserRoleDefault.Manager)]
        [HttpGet]
        public async Task<IActionResult> Delete(string CollectionId)
        {
            try
            {
                var collection = await _collectionApiService.GetCollectionById(CollectionId);
                if (collection is ApiErrorResult<CollectionVm> errorResult)
                {
                    List<string> listError = new List<string>();
                    if (collection.Message != null)
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
                return View(collection.ResultObj);
            }
            catch
            {
                TempData["ErrorToast"] = true;
                return View();
            }
        }
        [Authorize(Roles = DiamondLuxurySolution.Utilities.Constants.Systemconstant.UserRoleDefault.Manager)]
        [HttpPost]
        public async Task<IActionResult> Delete(DeleteCollectionRequest request)
        {
            try
            {
                var status = await _collectionApiService.DeleteCollection(request);
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
                return RedirectToAction("Index", "Collection");

            }
            catch
            {
                TempData["ErrorToast"] = true;
                return View();
            }
        }


    }
}

