﻿using DiamondLuxurySolution.ViewModel.Common;
using DiamondLuxurySolution.ViewModel.Models.Gem;
using DiamondLuxurySolution.ViewModel.Models.GemPriceList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondLuxurySolution.Application.Repository.GemPriceList
{
    public interface IGemPriceListRepo
    {
        public Task<ApiResult<List<GemPriceListVm>>> GetAll();
        public Task<ApiResult<bool>> CreateGemPriceList(CreateGemPriceListRequest request);
        public Task<ApiResult<bool>> UpdateGemPriceList(UpdateGemPriceListRequest request);
        public Task<ApiResult<bool>> DeleteGemPriceList(DeleteGemPriceListRequest request);
        public Task<ApiResult<GemPriceListVm>> GetGemPriceListById(int GemPriceListId);
        public Task<ApiResult<PageResult<GemPriceListVm>>> ViewGemPriceList(ViewGemPriceListRequest request);
    }
}
