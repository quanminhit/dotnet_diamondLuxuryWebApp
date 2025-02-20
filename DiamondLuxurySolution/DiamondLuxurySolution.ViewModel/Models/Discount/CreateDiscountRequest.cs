﻿using Azure.Core.Pipeline;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondLuxurySolution.ViewModel.Models.Discount
{
    public class CreateDiscountRequest
    {
        [Required(ErrorMessage = "Cần nhập tên mã giảm giá")]
        public string? DiscountName { get; set; }

        public string? Description { get; set; }

        [Required(ErrorMessage = "Cần nhập phần trăm chiết khấu")]
        public string? PercentSale { get; set; }
		[Required(ErrorMessage = "Cần nhập điểm bắt đầu chiết khấu")]
		public string? From { get; set; }
		[Required(ErrorMessage = "Cần nhập điểm đến chiết khấu")]
		public string? To { get; set; }

        public bool Status { get; set; }
    }
}
