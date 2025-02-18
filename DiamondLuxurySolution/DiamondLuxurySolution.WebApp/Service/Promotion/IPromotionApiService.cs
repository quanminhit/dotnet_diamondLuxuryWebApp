﻿using DiamondLuxurySolution.ViewModel.Common;
using DiamondLuxurySolution.ViewModel.Models.Promotion;

namespace DiamondLuxurySolution.WebApp.Service.Promotion
{
    public interface IPromotionApiService
    {
        public Task<ApiResult<List<PromotionVm>>> GetAll();
        public Task<ApiResult<List<PromotionVm>>> GetAllOnTime();

        public Task<ApiResult<PromotionVm>> GetPromotionById(Guid PromotionId);
        public Task<ApiResult<PageResult<PromotionVm>>> ViewPromotionInCustomer(ViewPromotionRequest request);

        public Task<ApiResult<PageResult<PromotionVm>>> ViewPromotionInManager(ViewPromotionRequest request);
    }
}
