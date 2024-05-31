﻿using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondLuxurySolution.ViewModel.Models.Material
{
    public class UpdateMaterialRequest
    {
        public Guid MaterialId { get; set; } 

        [Required(ErrorMessage = "Yêu cầu tên của vat lieu")]
        public string? MaterialName { get; set; }

        public string? Description { get; set; }

        public string? Color { get; set; }
        public decimal? Weight { get; set; }

        public IFormFile? MaterialImage { get; set; }

        public bool Status { get; set; }
        public DateTime EffectDate { get; set; }

        public decimal? Price { get; set; }
    }
}
