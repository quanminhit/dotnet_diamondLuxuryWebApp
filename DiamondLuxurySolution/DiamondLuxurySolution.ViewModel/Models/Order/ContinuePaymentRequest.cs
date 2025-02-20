﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondLuxurySolution.ViewModel.Models.Order
{
    public class ContinuePaymentRequest
    {
        public string OrderId { get; set; }
        public decimal? PaidTheRest { get; set; }
        public Guid PaymentId { get; set; }
        public string? TransactionStatus { get; set; }
        public string? Message { get; set; }
    }
}
