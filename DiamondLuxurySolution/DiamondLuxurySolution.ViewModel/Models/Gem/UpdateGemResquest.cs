﻿using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondLuxurySolution.ViewModel.Models.Gem
{
    public class UpdateGemResquest
    {
        public Guid GemId { get; set; }

        public string GemName { get; set; } = null!;

        public IFormFile? ProportionImage { get; set; }

        public string? Symetry { get; set; }

        public string? Polish { get; set; }

        public decimal Price { get; set; }

        public bool IsOrigin { get; set; }

        public bool IsMain { get; set; }

        public bool Fluoresence { get; set; }

        public DateTime AcquisitionDate { get; set; }

        public bool Active { get; set; }
    }
}
