﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondLuxurySolution.ViewModel.Models.User.Staff
{
    public class ChangePasswordStaffRequest
    {
        public Guid StaffId { get; set; }
        public string OldPassword { get; set; }

        public string NewPassword { get; set; }
    }
}
