﻿using Azure.Core;
using DiamondLuxurySolution.Data.EF;
using DiamondLuxurySolution.Data.Entities;
using DiamondLuxurySolution.Utilities.Helper;
using DiamondLuxurySolution.ViewModel.Common;
using DiamondLuxurySolution.ViewModel.Models.User.Customer;
using Firebase.Auth;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using PagedList;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Policy;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static DiamondLuxurySolution.Utilities.Constants.Systemconstant;

namespace DiamondLuxurySolution.Application.Repository.User.Customer
{
    public class CustomerRepo : ICustomerRepo
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<AppRole> _roleManager;
        private readonly LuxuryDiamondShopContext _context;
        private readonly SignInManager<AppUser> _signInManager;

        public CustomerRepo(LuxuryDiamondShopContext context, UserManager<AppUser> userManager, RoleManager<AppRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _context = context;
        }

        public async Task<ApiResult<bool>> ChangePasswordCustomer(ChangePasswordCustomerRequest request)
        {
            var user = await _userManager.FindByIdAsync(request.CustomerId.ToString());
            if (user == null)
            {
                return new ApiErrorResult<bool>("Khách hàng không tồn tại");
            }
            var comfirmPassword = await _userManager.CheckPasswordAsync(user, request.OldPassword);
            if (comfirmPassword == false)
            {
                return new ApiErrorResult<bool>("Sai mật khẩu hiện tại");
            }
            var changePasswordResult = await _userManager.ChangePasswordAsync(user, request.OldPassword, request.NewPassword);

            if (!changePasswordResult.Succeeded)
            {
                var errorApi = new ApiErrorResult<bool>("Lỗi thông tin");
                errorApi.ValidationErrors = new List<string>();
                foreach (var item in changePasswordResult.Errors)
                {
                    errorApi.ValidationErrors.Add(item.Description);
                }
                return errorApi;
            }
            user.LastChangePasswordTime = DateTime.Now;

            var statusUser = await _userManager.UpdateAsync(user);
            if (!statusUser.Succeeded)
            {
                return new ApiErrorResult<bool>("Lỗi hệ thống, thay đổi mật khẩu thất bại vui lòng thử lại");
            }
            return new ApiSuccessResult<bool>("Cập nhật mật khẩu thành công");
        }

        public async Task<ApiResult<List<int>>> CountAllCustomerInYear()
        {
            try
            {
                var currentYear = DateTime.UtcNow.Year;
                var customerCounts = await _userManager.Users
                    .Where(user => user.DateCreated.Value.Year == currentYear)
                    .GroupBy(user => user.DateCreated.Value.Month)
                    .Select(g => new { Month = g.Key, Count = g.Count() })
                    .ToListAsync();

                // Initialize a list with 12 zeros (for each month)
                var monthlyCounts = Enumerable.Repeat(0, 12).ToList();

                // Fill the counts in the correct month positions
                foreach (var customerCount in customerCounts)
                {
                    monthlyCounts[customerCount.Month - 1] = customerCount.Count;
                }

                return new ApiSuccessResult<List<int>>(monthlyCounts, "Success");
            }
            catch (Exception ex)
            {
                // Handle exception, log the error
                return new ApiErrorResult<List<int>>($"Error: {ex.Message}");
            }
        }

        public async Task<ApiResult<bool>> DeleteCustomer(Guid CustomerId)
        {
            var user = await _userManager.FindByIdAsync(CustomerId.ToString());
            if (user == null)
            {
                return new ApiErrorResult<bool>("Khách hàng không tồn tại");
            }
            var statusUser = await _userManager.DeleteAsync(user);
            if (!statusUser.Succeeded)
            {
                return new ApiErrorResult<bool>("Lỗi hệ thống,xóa khách hàng thất bại vui lòng thử lại");
            }
            return new ApiSuccessResult<bool>(true, "Success");
        }

        public async Task<ApiResult<bool>> ForgotpasswordCustomerChange(ForgotPasswordCustomerChangeRequest request)
        {
            var user = await _userManager.FindByEmailAsync(request.Email.Trim().ToString());
            if (user == null)
            {
                return new ApiErrorResult<bool>("Email không tồn tại");
            }
            if (!request.NewPassword.Trim().Equals(request.ConfirmPassword.Trim()))
            {
                return new ApiErrorResult<bool>("Mật khẩu không trùng khớp");
            }

            if (!user.Status.Equals(DiamondLuxurySolution.Utilities.Constants.Systemconstant.StaffStatus.ChangePasswordRequest.ToString()))
            {
                return new ApiErrorResult<bool>("Không có yêu câu thay đổi mật khẩu");
            }
            var removePasswordResult = await _userManager.RemovePasswordAsync(user);
            if (!removePasswordResult.Succeeded)
            {
                return new ApiErrorResult<bool>("Xóa mật khẩu cũ thất bại");
            }

            var addPasswordResult = await _userManager.AddPasswordAsync(user, request.NewPassword.Trim());
            if (!addPasswordResult.Succeeded)
            {
                var errorApi = new ApiErrorResult<bool>("Lỗi thông tin");
                errorApi.ValidationErrors = new List<string>();
                foreach (var item in addPasswordResult.Errors)
                {
                    errorApi.ValidationErrors.Add(item.Description);
                }
                return errorApi;
            }
            if (!addPasswordResult.Succeeded)
            {

                return new ApiErrorResult<bool>("Thêm mật khẩu mới thất bại");
            }
            user.LastChangePasswordTime = DateTime.Now;
            user.Status = DiamondLuxurySolution.Utilities.Constants.Systemconstant.CustomerStatus.Active.ToString();
            var statusUser = await _userManager.UpdateAsync(user);
            if (!statusUser.Succeeded)
            {
                return new ApiErrorResult<bool>("Lỗi hệ thống, cập nhật thông tin thất bại vui lòng thử lại");
            }

            return new ApiSuccessResult<bool>(true, "Thay đổi mật khẩu thành công");
        }

        public async Task<ApiResult<string>> ForgotpasswordCustomerSendCode(string Email)
        {
            var user = await _userManager.FindByEmailAsync(Email.Trim().ToString());

            if (user == null)
            {
                return new ApiErrorResult<string>("Email không tồn tại");
            }
            var roles = await _userManager.GetRolesAsync(user);
            foreach (var role in roles)
            {
                if (role.Equals(DiamondLuxurySolution.Utilities.Constants.Systemconstant.UserRoleDefault.Customer))
                {
                    continue;
                }
                return new ApiErrorResult<string>("Không hợp lệ");

            }
            Random rd = new Random();
            string code = "" + rd.Next(0, 9).ToString() + rd.Next(0, 9).ToString() + rd.Next(0, 9).ToString() + rd.Next(0, 9).ToString() + rd.Next(0, 9).ToString() + rd.Next(0, 9).ToString();

            user.Status = DiamondLuxurySolution.Utilities.Constants.Systemconstant.CustomerStatus.ChangePasswordRequest.ToString();
            var statusUser = await _userManager.UpdateAsync(user);
            if (!statusUser.Succeeded)
            {
                return new ApiErrorResult<string>("Lỗi hệ thống, cập nhật thông tin thất bại vui lòng thử lại");
            }


            SendEmailForgot(code, user.Email);

            return new ApiSuccessResult<string>(code, "Success");
        }
        public void SendEmailForgot(string code, string toEmail)
        {
            // Correct relative path from current directory to the HTML file
            string relativePath = @"..\..\DiamondLuxurySolution\DiamondLuxurySolution.Utilities\FormSendEmail\SendCodeCustomer.html";
            // Combine the relative path with the current directory to get the full path
            string path = Path.GetFullPath(relativePath);

            if (!System.IO.File.Exists(path))
            {
                return;
            }
            string contentCustomer = System.IO.File.ReadAllText(path);
            contentCustomer = contentCustomer.Replace("{{VerifyCode}}", code);
            DoingMail.SendMail("LuxuryDiamond", "Yêu cầu thay đổi mật khẩu", contentCustomer, toEmail);
        }
        public async Task<ApiResult<CustomerVm>> GetCustomerByEmail(string Email)
        {
            var user = await _userManager.FindByEmailAsync(Email.ToString());
            if (user == null)
            {
                return new ApiErrorResult<CustomerVm>("Khách hàng không tồn tại");
            }
            var customerVm = new CustomerVm()
            {
                CustomerId = user.Id,
                Dob = user.Dob != null ? (DateTime)user.Dob : DateTime.MinValue,
                FullName = user.Fullname,
                PhoneNumber = user.PhoneNumber,
                Email = user.Email,
                Status = user.Status,
                Address = user.Address
            };
            var listRoleOfUser = await _userManager.GetRolesAsync(user);

            if (listRoleOfUser.Count > 0)
            {
                customerVm.ListRoleName = new List<string>();
                foreach (var role in listRoleOfUser)
                {
                    customerVm.ListRoleName.Add(role);
                }
            }
            return new ApiSuccessResult<CustomerVm>(customerVm, "Success");
        }

        public async Task<ApiResult<CustomerVm>> GetCustomerById(Guid CustomerId)
        {
            var user = await _userManager.FindByIdAsync(CustomerId.ToString());
            if (user == null)
            {
                return new ApiErrorResult<CustomerVm>("Khách hàng không tồn tại");
            }
            var customerVm = new CustomerVm()
            {
                CustomerId = user.Id,
                Dob = user.Dob != null ? (DateTime)user.Dob : DateTime.MinValue,
                FullName = user.Fullname,
                PhoneNumber = user.PhoneNumber,
                Email = user.Email,
                Status = user.Status,
                Address = user.Address
            };
            var listRoleOfUser = await _userManager.GetRolesAsync(user);

            if (listRoleOfUser.Count > 0)
            {
                customerVm.ListRoleName = new List<string>();
                foreach (var role in listRoleOfUser)
                {
                    customerVm.ListRoleName.Add(role);
                }
            }
            return new ApiSuccessResult<CustomerVm>(customerVm, "Success");
        }

        public async Task<ApiResult<bool>> Login(LoginCustomerRequest request)
        {

            if (string.IsNullOrEmpty(request.Email) || string.IsNullOrEmpty(request.Password))
            {
                return new ApiErrorResult<bool>("Tải khoản hoặc mật khẩu không đúng");
            }
            if (!DiamondLuxurySolution.Utilities.Helper.CheckValidInput.IsValidEmail(request.Email))
            {
                return new ApiErrorResult<bool>("Email không hợp lệ");
            }
            var user = await _userManager.FindByEmailAsync(request.Email.Trim());
            if (user == null)
            {
                return new ApiErrorResult<bool>("Tải khoản hoặc mật khẩu không đúng");
            }
            var role = await _userManager.GetRolesAsync(user);
            if (!role.Contains(DiamondLuxurySolution.Utilities.Constants.Systemconstant.UserRoleDefault.Customer.ToString()))
            {
                return new ApiErrorResult<bool>("Tải khoản hoặc mật khẩu không đúng");
            }
            var userPasswordConfirm = await _userManager.CheckPasswordAsync(user, request.Password.Trim());
            if (userPasswordConfirm == false)
            {
                return new ApiErrorResult<bool>("Tải khoản hoặc mật khẩu không đúng");
            }

            return new ApiSuccessResult<bool>(true, "Success");
        }

        public async Task<ApiResult<bool>> Register(RegisterCustomerAccountRequest request)
        {
            var userFindByEmail = await _userManager.FindByEmailAsync(request.Email);
            if (userFindByEmail != null)
            {
                return new ApiErrorResult<bool>("Email đã tồn tại");
            }
            List<string> errorList = new List<string>();
            if (!request.Password.Equals(request.ConfirmPassword))
            {
                errorList.Add("Password không trùng khớp");
            }

            if (DiamondLuxurySolution.Utilities.Helper.CheckValidInput.ContainsLetters(request.PhoneNumber))
            {
                errorList.Add("Số điện thoại không hợp lệ");
            }

            #region Check lỗi phoneNumbers
            if (string.IsNullOrWhiteSpace(request.PhoneNumber))
            {
                errorList.Add("Vui lòng nhập số điện thoại");
            }
            else
            {
                if (!Regex.IsMatch(request.PhoneNumber, "^(09|03|07|08|05)[0-9]{8,9}$"))
                {
                    errorList.Add("Số điện thoại không hợp lệ");
                }
            }
            #endregion End

            if (!DiamondLuxurySolution.Utilities.Helper.CheckValidInput.IsValidEmail(request.Email))
            {
                errorList.Add("Email không hợp lệ");
            }
            if (errorList.Any())
            {
                return new ApiErrorResult<bool>("Không hợp lệ", errorList);
            }
            Random rd = new Random();
            string username = "Cus" + rd.Next(0, 9).ToString() + rd.Next(0, 9).ToString() + rd.Next(0, 9).ToString() + rd.Next(0, 9).ToString() + rd.Next(0, 9).ToString() + rd.Next(0, 9).ToString();

            var user = new AppUser()
            {
                Fullname = request.FullName.Trim(),
                Email = request.Email.Trim(),
                Dob = request.Dob,
                PhoneNumber = request.PhoneNumber.Trim(),
                UserName = username,
                Address = "",
                Point = 0,
                DateCreated = DateTime.Now
            };
            user.Status = DiamondLuxurySolution.Utilities.Constants.Systemconstant.CustomerStatus.New.ToString();
            var status = await _userManager.CreateAsync(user, request.Password);
            if (!status.Succeeded)
            {
                var errorApi = new ApiErrorResult<bool>("Lỗi đăng kí");
                errorApi.ValidationErrors = new List<string>();
                foreach (var item in status.Errors)
                {
                    errorApi.ValidationErrors.Add(item.Description);
                }
                return errorApi;
            }


            var customerRole = new AppRole()
            {
                Name = DiamondLuxurySolution.Utilities.Constants.Systemconstant.UserRoleDefault.Customer,
                Description = "Khách hàng mặc định"
            };
            var role = await _roleManager.FindByNameAsync(customerRole.Name);
            if (role == null)
            {
                var statusCreateRole = await _roleManager.CreateAsync(customerRole);
                if (!status.Succeeded)
                {
                    return new ApiErrorResult<bool>("Lỗi hệ thống, không thể tạo role vui lòng thử lại");
                }
            }

            var statusAddRole = await _userManager.AddToRoleAsync(user, customerRole.Name);
            if (!statusAddRole.Succeeded)
            {
                return new ApiErrorResult<bool>("Lỗi hệ thống, không thể thêm role vào tải khoản vui lòng thử lại");
            }


            SendEmail(request.FullName, request.Email);

            return new ApiSuccessResult<bool>(true, "Đăng kí thành công");
        }
        public void SendEmail(string code, string toEmail)
        {
            // Correct relative path from current directory to the HTML file
            string relativePath = @"..\..\DiamondLuxurySolution\DiamondLuxurySolution.Utilities\FormSendEmail\Welcome.html";
            // Combine the relative path with the current directory to get the full path
            string path = Path.GetFullPath(relativePath);

            if (!System.IO.File.Exists(path))
            {
                return;
            }
            string contentCustomer = System.IO.File.ReadAllText(path);
            contentCustomer = contentCustomer.Replace("{{Username}}", code);
            DoingMail.SendMail("LuxuryDiamond", "Chào mứng đến với Diamond Luxury", contentCustomer, toEmail);
        }
        public async Task<ApiResult<bool>> UpdateCustomerAccount(UpdateCustomerRequest request)
        {
            var user = await _userManager.FindByIdAsync(request.CustomerId.ToString());
            List<string> errorList = new List<string>();
            if (user == null)
            {
                return new ApiErrorResult<bool>("Khách hàng không tồn tại");
            }
            
            if (!string.IsNullOrEmpty(request.PhoneNumber))
            {
                if (DiamondLuxurySolution.Utilities.Helper.CheckValidInput.ContainsLetters(request.PhoneNumber.Trim()))
                {
                    errorList.Add("Số điện thoại không hợp lệ");
                }
            }
            if (!string.IsNullOrEmpty(request.PhoneNumber))
            {
                if (!DiamondLuxurySolution.Utilities.Helper.CheckValidInput.ValidPhoneNumber(request.PhoneNumber.Trim()))
                {
                    errorList.Add("Số điện thoại không hợp lệ");
                }
            }
            
            if (errorList.Any())
            {
                return new ApiErrorResult<bool>("Không hợp lệ", errorList);
            }
            if (!string.IsNullOrWhiteSpace(request.PhoneNumber))
            {
                user.PhoneNumber = request.PhoneNumber.Trim();
            }
            if (request.Dob != DateTime.MinValue)
            {
                user.Dob = request.Dob;
            }
            if (!string.IsNullOrWhiteSpace(request.Address))
            {
                user.Address = request.Address.Trim();
            }

            var statusUser = await _userManager.UpdateAsync(user);
            if (!statusUser.Succeeded)
            {
                return new ApiErrorResult<bool>("Lỗi hệ thống, cập nhật thông tin thất bại vui lòng thử lại");
            }
            // Update Password
            if (!string.IsNullOrEmpty(request.OldPassword) && !string.IsNullOrEmpty(request.NewPassword) && !string.IsNullOrEmpty(request.ConfirmPassword))
            {
                var comfirmPassword = await _userManager.CheckPasswordAsync(user, request.OldPassword);
                if (comfirmPassword == false)
                {
                    return new ApiErrorResult<bool>("Sai mật khẩu hiện tại");
                }
                var changePasswordResult = await _userManager.ChangePasswordAsync(user, request.OldPassword, request.NewPassword);

                if (!changePasswordResult.Succeeded)
                {
                    var errorApi = new ApiErrorResult<bool>("Lỗi thông tin");
                    errorApi.ValidationErrors = new List<string>();
                    foreach (var item in changePasswordResult.Errors)
                    {
                        errorApi.ValidationErrors.Add(item.Description);
                    }
                    return errorApi;
                }
                user.LastChangePasswordTime = DateTime.Now;

                var statusUserChangePassword = await _userManager.UpdateAsync(user);
                if (!statusUserChangePassword.Succeeded)
                {
                    return new ApiErrorResult<bool>("Lỗi hệ thống, thay đổi mật khẩu thất bại vui lòng thử lại");
                }
                return new ApiSuccessResult<bool>("Cập nhật mật khẩu thành công");
            }







            return new ApiSuccessResult<bool>("Cập nhật thông tin thành công");

        }


    }
}
