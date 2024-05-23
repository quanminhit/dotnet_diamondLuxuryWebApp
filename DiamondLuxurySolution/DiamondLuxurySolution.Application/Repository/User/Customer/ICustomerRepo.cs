﻿using DiamondLuxurySolution.ViewModel.Common;
using DiamondLuxurySolution.ViewModel.Models.User.Customer;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondLuxurySolution.Application.Repository.User.Customer
{
    public  interface ICustomerRepo
    {
        public Task<ApiResult<bool>> Login(LoginCustomerRequest request);
        public Task<ApiResult<bool>> Register(RegisterCustomerAccountRequest request);
        public Task<ApiResult<PageResult<CustomerVm>>> ViewCustomerPagination(ViewCustomerPaginationRequest request);
        public Task<ApiResult<bool>> UpdateCustomerAccount(UpdateCustomerRequest request);
        public Task<ApiResult<CustomerVm>> GetCustomerById(Guid CustomerId);
        public Task<ApiResult<bool>> DeleteCustomer(Guid CustomerId);
        public Task<ApiResult<bool>> ChangePasswordCustomer(ChangePasswordCustomerRequest request);





    }
}
