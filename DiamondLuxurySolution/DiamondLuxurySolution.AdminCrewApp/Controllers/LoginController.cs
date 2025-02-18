﻿using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using DiamondLuxurySolution.ViewModel.Models.User.Staff;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Logging;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using DiamondLuxurySolution.Data.Entities;
using Microsoft.AspNetCore.Identity;
using DiamondLuxurySolution.AdminCrewApp.Service.Login;
using Azure.Core;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using DiamondLuxurySolution.ViewModel.Common;
using DiamondLuxurySolution.AdminCrewApp.Service.Staff;
using DiamondLuxurySolution.AdminCrewApp.Models;

namespace DiamondLuxurySolution.AdminCrewApp.Controllers
{
    [AllowAnonymous]
    public class LoginController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly ILoginApiService _loginApiService;
        private readonly IStaffApiService _staffApiService;

        public LoginController(IConfiguration configuration, ILoginApiService loginApiService, IStaffApiService staffApiService)
        {
            _configuration = configuration;
            _staffApiService = staffApiService;
            _loginApiService = loginApiService;
        }


        [HttpGet]
        public async Task<IActionResult> Index()
        {
            return View();
        }


        [HttpPost]
        public async Task<IActionResult> Index(LoginStaffRequest request)
        {


            var apiResult = await _loginApiService.LoginStaff(request);

            if (apiResult.IsSuccessed)
            {
                var userPrincipal = this.ValidateToken(apiResult.ResultObj);
                var authProperties = new Microsoft.AspNetCore.Authentication.AuthenticationProperties
                {
                    ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(10),
                    IsPersistent = false
                };
                HttpContext.Session.SetString("token", apiResult.ResultObj);
                await HttpContext.SignInAsync(
                            CookieAuthenticationDefaults.AuthenticationScheme,
                            userPrincipal,
                            authProperties);


                var user = await _staffApiService.GetStaffByUsername(request.UserName);
                var userId = user.ResultObj.StaffId;

                HttpContext.Session.SetString(DiamondLuxurySolution.Utilities.Constants.Systemconstant.AppSettings.USER_ID, userId.ToString());
                HttpContext.Session.SetString(DiamondLuxurySolution.Utilities.Constants.Systemconstant.AppSettings.USER_NAME, user.ResultObj.FullName);
                HttpContext.Session.SetString(DiamondLuxurySolution.Utilities.Constants.Systemconstant.AppSettings.USER_EMAIL, user.ResultObj.Email);
                HttpContext.Session.SetString(DiamondLuxurySolution.Utilities.Constants.Systemconstant.AppSettings.USER_IMAGE, string.IsNullOrEmpty(user.ResultObj.Image) ? "" : user.ResultObj.Image);

                StaffSessionHelper.SetObjectAsJson("Staff", user.ResultObj);


                if (user.ResultObj.ListRoleName.Contains(DiamondLuxurySolution.Utilities.Constants.Systemconstant.UserRoleDefault.Admin)){
                    return RedirectToAction("Admin", "Home");
                }
                if (user.ResultObj.ListRoleName.Contains(DiamondLuxurySolution.Utilities.Constants.Systemconstant.UserRoleDefault.Manager)){
                    return RedirectToAction("Manager", "Home");
                }
                if (user.ResultObj.ListRoleName.Contains(DiamondLuxurySolution.Utilities.Constants.Systemconstant.UserRoleDefault.SalesStaff)){
                    return RedirectToAction("Index", "Product");
                }
                if (user.ResultObj.ListRoleName.Contains(DiamondLuxurySolution.Utilities.Constants.Systemconstant.UserRoleDefault.DeliveryStaff)){
                    return RedirectToAction("IndexOrder", "DeliveryStaff");
                }

            }

            ViewBag.Error = "Tài khoản hoặc mật khẩu không đúng";
            return View((object)request.UserName);
        }

        [HttpGet]
        public async Task<IActionResult> ForgotPassword()
        {

            return View();
        }
        [HttpPost]
        public async Task<IActionResult> ForgotPassword(string Username)
        {
            var apiResult = await _loginApiService.ForgotpasswordStaffSendCode(Username);
            if (!apiResult.IsSuccessed)
            {
                ViewBag.Error = apiResult.Message;
                return View((object)Username);
            }
            HttpContext.Session.SetString("Code", apiResult.ResultObj.ToString());
            HttpContext.Session.SetString("Username", Username.ToString());

            return RedirectToAction("VerifyCode", "Login");
        }
        [HttpGet]
        public async Task<IActionResult> VerifyCode()
        {

            return View();
        }
        [HttpPost]
        public async Task<IActionResult> VerifyCode(string code)
        {
            if (HttpContext.Session.GetString("Code").ToString().Equals(code.ToString()))
            {
                HttpContext.Session.Remove("Code");
                return RedirectToAction("ChangePassword", "Login");
            }
            ViewBag.Error = "Sai mã xác thực";
            return View((object)code);
        }

        [HttpGet]
        public async Task<IActionResult> ChangePassword()
        {

            return View();
        }
        [HttpPost]
        public async Task<IActionResult> ChangePassword(ForgotPasswordStaffChangeRequest request)
        {
            request.Username = HttpContext.Session.GetString("Username").ToString();
            var status = await _loginApiService.ForgotpassworStaffdChange(request);
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

            return RedirectToAction("Index", "Login");
        }

        private ClaimsPrincipal ValidateToken(string jwtToken)
        {
            IdentityModelEventSource.ShowPII = true;

            TokenValidationParameters validationParameters = new TokenValidationParameters
            {
                ValidateLifetime = true,
                ValidAudience = _configuration["Jwt:Audience"],
                ValidIssuer = _configuration["Jwt:Issuer"],
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]))
            };

            SecurityToken validatedToken;
            JwtSecurityTokenHandler handler = new JwtSecurityTokenHandler();
            ClaimsPrincipal principal = handler.ValidateToken(jwtToken, validationParameters, out validatedToken);

            return principal;
        }






    }

}
