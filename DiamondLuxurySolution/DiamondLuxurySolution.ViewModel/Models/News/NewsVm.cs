﻿using DiamondLuxurySolution.Data.Entities;
using DiamondLuxurySolution.ViewModel.Models.User.Staff;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondLuxurySolution.ViewModel.Models.News
{
    public class NewsVm
    {
        public int NewsId { get; set; }
        public string? NewName { get; set; }

        public string? Title { get; set; }

        public string? Image { get; set; }

        public string? Description { get; set; }

        public DateTime? DateCreated { get; set; }

        public DateTime? DateModified { get; set; }

        public bool? IsOutstanding { get; set; }

        public virtual StaffVm? Writer { get; set; }
    }
}
