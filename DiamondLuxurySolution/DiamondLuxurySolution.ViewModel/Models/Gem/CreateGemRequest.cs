﻿using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondLuxurySolution.ViewModel.Models.Gem
{
    public class CreateGemRequest
    {
        public string GemName { get; set; } = null!;
        public IFormFile ProportionImage { get; set; } = null!;

        public string Symetry { get; set; } = null!;

        public string Polish { get; set; } = null!;

        public decimal Price { get; set; }

        public bool IsOrigin { get; set; }

        public bool _4c { get; set; }

        public bool IsMain { get; set; }

        public bool Fluoresence { get; set; }

        public bool Active { get; set; }
    }
}
