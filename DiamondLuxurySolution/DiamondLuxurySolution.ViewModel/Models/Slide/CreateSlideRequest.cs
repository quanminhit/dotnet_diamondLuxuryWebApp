﻿using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondLuxurySolution.ViewModel.Models.Slide
{
    public class CreateSlideRequest
    {
        public string? SlideName { get; set; }

        public string? Description { get; set; }

        public string? SlideUrl { get; set; }

        public IFormFile? SlideImage { get; set; }

        public bool Status { get; set; }
    }
}
