﻿using DiamondLuxurySolution.AdminCrewApp.Service.Role;
using DiamondLuxurySolution.AdminCrewApp.Service.Staff;
using DiamondLuxurySolution.ViewModel.Common;
using DiamondLuxurySolution.ViewModel.Models.User.Staff;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static DiamondLuxurySolution.Utilities.Constants.Systemconstant;

namespace DiamondLuxurySolution.AdminCrewApp.Controllers
{
    [Authorize(Roles = DiamondLuxurySolution.Utilities.Constants.Systemconstant.UserRoleDefault.Admin + ", " + DiamondLuxurySolution.Utilities.Constants.Systemconstant.UserRoleDefault.Manager)]


    public class SaleStaffController : BaseController
    {
        private readonly IStaffApiService _staffApiService;
        private readonly IRoleApiService _roleApiService;

        public SaleStaffController(IStaffApiService staffApiService, IRoleApiService roleApiService)
        {
            _staffApiService = staffApiService;
            _roleApiService = roleApiService;
        }

        [HttpGet]
        public async Task<IActionResult> Index(ViewStaffPaginationCommonRequest request)
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

                var Staff = await _staffApiService.ViewSalesStaffPagination(request);
                return View(Staff.ResultObj);
            }
            catch
            {
                return View();
            }
        }
        [HttpGet]
        public async Task<IActionResult> Detail(Guid StaffId)
        {
            try
            {
                var status = await _staffApiService.GetStaffById(StaffId);
                if (status is ApiErrorResult<StaffVm> errorResult)
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
        public async Task<IActionResult> Edit(Guid StaffId)
        {
            try
            {
                var statuses = Enum.GetValues(typeof(StaffStatus)).Cast<StaffStatus>().ToList();
                ViewBag.ListStatus = statuses;

                var listRoleAll = await _roleApiService.GetRolesForView();
                ViewBag.ListRoleAll = listRoleAll.ResultObj.ToList();

                var Staff = await _staffApiService.GetStaffById(StaffId);
                if (Staff is ApiErrorResult<StaffVm> errorResult)
                {
                    List<string> listError = new List<string>();
                    if (Staff.Message != null)
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
                var listRoleName = new List<string>();

                StaffVm staffVm = new StaffVm()
                {
                    Address = Staff.ResultObj.Address,
                    CitizenIDCard = Staff.ResultObj.CitizenIDCard,
                    StaffId = Staff.ResultObj.StaffId,
                    Dob = (DateTime)Staff.ResultObj.Dob,
                    Email = Staff.ResultObj.Email,
                    FullName = Staff.ResultObj.FullName,
                    Image = Staff.ResultObj.Image,
                    PhoneNumber = Staff.ResultObj.PhoneNumber,
                    ListRoleName = Staff.ResultObj.ListRoleName,
                    Status = Staff.ResultObj.Status,
                    Username = Staff.ResultObj.Username
                };
                return View(staffVm);
            }
            catch
            {
                TempData["ErrorToast"] = true;
                return View();
            }
        }
        [HttpPost]
        public async Task<IActionResult> Edit(UpdateStaffAccountRequest request)
        {
            try
            {
                var staff = await _staffApiService.GetStaffById(request.StaffId);

                var statuses = Enum.GetValues(typeof(StaffStatus)).Cast<StaffStatus>().ToList();
                ViewBag.ListStatus = statuses;

                var listRoleAll = await _roleApiService.GetRolesForView();
                ViewBag.ListRoleAll = listRoleAll.ResultObj.ToList();


                if (!ModelState.IsValid)
                {
                    var listRoleName = new List<string>();
                    foreach (var item in request.RoleId)
                    {
                        var role = _roleApiService.GetRoleById(item);
                        listRoleName.Add(role.Result.ResultObj.Name);
                    }
                    StaffVm staffVm = new StaffVm()
                    {
                        Username = request.Username,
                        Address = request.Address,
                        CitizenIDCard = request.CitizenIDCard,
                        Dob = request.Dob,
                        Email = request.Email,
                        FullName = request.FullName,
                        ListRoleName = request.ListRoleName,
                        PhoneNumber = request.PhoneNumber,
                        RoleId = request.RoleId,
                        StaffId = request.StaffId,
                        Status = request.Status,
                        Image = staff.ResultObj.Image,
                    };
                    if (listRoleName.Count > 0)
                    {
                        staffVm.ListRoleName = listRoleName;
                    }
                    TempData["WarningToast"] = true;
                    return View(staffVm);
                }


                var status = await _staffApiService.UpdateStaffAccount(request);
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
                    var listRoleName = new List<string>();
                    foreach (var item in request.RoleId)
                    {
                        var role = _roleApiService.GetRoleById(item);
                        listRoleName.Add(role.Result.ResultObj.Name);
                    }
                    StaffVm staffVm = new StaffVm()
                    {
                        Username = request.Username,
                        Address = request.Address,
                        CitizenIDCard = request.CitizenIDCard,
                        Dob = request.Dob,
                        Email = request.Email,
                        FullName = request.FullName,
                        ListRoleName = request.ListRoleName,
                        PhoneNumber = request.PhoneNumber,
                        RoleId = request.RoleId,
                        StaffId = request.StaffId,
                        Status = request.Status,
                        Image = staff.ResultObj.Image,
                    };
                    if (listRoleName.Count > 0)
                    {
                        staffVm.ListRoleName = listRoleName;
                    }
                    TempData["WarningToast"] = true;
                    return View(staffVm);

                }
                TempData["SuccessToast"] = true;
                return RedirectToAction("Index", "SaleStaff");
            }
            catch
            {
                TempData["WarningToast"] = true;
                return View(request);
            }
        }



        [HttpGet]
        public async Task<IActionResult> Delete(Guid StaffId)
        {
            try
            {
                var Staff = await _staffApiService.GetStaffById(StaffId);
                if (Staff is ApiErrorResult<StaffVm> errorResult)
                {
                    List<string> listError = new List<string>();
                    if (Staff.Message != null)
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
                return View(Staff.ResultObj);
            }
            catch
            {
                TempData["ErrorToast"] = true;
                return View();
            }
        }

        [HttpPost]
        public async Task<IActionResult> Delete(DeleteStaffAccountRequest request)
        {
            try
            {
                var status = await _staffApiService.DeleteStaff(request.StaffId);
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
                    TempData["ErrorToast"] = true;
                    ViewBag.Errors = listError;
                    return View();

                }
                TempData["SuccessToast"] = true;
                return RedirectToAction("Index", "SaleStaff");

            }
            catch
            {
                TempData["ErrorToast"] = true;
                return View();
            }
        }

    }
}
