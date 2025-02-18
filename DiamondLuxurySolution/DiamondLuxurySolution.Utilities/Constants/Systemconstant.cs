﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondLuxurySolution.Utilities.Constants
{
    public class Systemconstant
    {
        public class AppSettings
        {
            public const string BaseAddress = "Address:Base";
            public const int PAGE_SIZE = 12;
            public const string USER_ID = "UserId";
            public const string USER_EMAIL = "UserEmail";
            public const string USER_NAME = "Username";
            public const string USER_IMAGE = "UserImage";
            public const string CUSTOMER_NAME = "CusName";
            public const string CUSTOMER_ID = "CusId";
            public const string FACEBOOK_PLATFORM = "Facebook";
            public const string GOOGLE_PLATFORM = "Google";
            public const string DEFAULT_PLATFORM = "Default";
            public const string PLATFORM = "Platform";
        }


        public class UserRoleDefault
        {
            public const string Customer = "Khách hàng";
            public const string Manager = "Quản lý";
            public const string SalesStaff = "Nhân viên bán hàng";
            public const string DeliveryStaff = "Nhân viên giao hàng";
            public const string Admin = "Quản trị viên";
        }

        public class KnowledgeNewsCategoryNameDefault
        {
            public const string DiamondKnowledgeNewsCategoryName = "Kiến Thức Kim Cương";
            public const string JewelryKnowledgeNewsCategoryName = "Kiến Thức Trang Sức";
        }


        public class CategoryNameDefault
		{
			public const string Diamond = "Kim Cương";
			public const string WeddingJewelry = "Trang Sức Cưới";
			public const string Ring = "Nhẫn";
			public const string Necklace = "Dây Chuyền";
			public const string Earring = "Hoa Tai";
			public const string Bracelet = "Vòng Tay";
			public const string Bangles = "Lắc Tay";
		}

		public class SlideNameDefault
		{
			public const string MainSlide = "Slide";
			public const string Diamond = "Kim Cương";
			public const string WeddingJewelryRing = "Nhẫn Cưới";
			public const string RingMan = "Nhẫn Nam";
			public const string RingWoman = "Nhẫn Nữ";
			public const string Earring = "Hoa Tai";
			public const string Necklace = "Dây Chuyền";
		}

        public class MaterialNameDefault
        {
            public const string Gold10K = "Vàng 10K";
            public const string Gold14K = "Vàng 14K";
            public const string Gold18K = "Vàng 18K";
            public const string Platium850 = "Platium 850";
            public const string Platium950 = "Platium 950";
        }

		public enum OrderStatus
        {
            InProgress,
            Confirmed,
            Shipping,
            Success,
            Canceled,
            ProcessOfPayment,
            Deposit,
        }
        public enum Status
        {
            Inactive,
            Active
        }
        public enum ProductStatus
        {
            Selling,
            OutOfStock,
            Sales,
            Coming
        }
        public enum TransactionStatus
        {
            Success,
            Failed,
            Waiting
        }

        public enum WarrantyDetailStatus
        {
            Received,
            UnderReview,
            Approved,
            Denied,
            InRepair,
            RepairCompleted,
            QualityCheck,
            CustomerNotified,
            RefundProcessed,
            Success
        }

        public enum StaffStatus
        {
            Active,         // Nhân viên đang hoạt động
            Inactive,       // Nhân viên không hoạt động
            OnLeave,        // Nhân viên đang nghỉ phép
            Suspended,      // Nhân viên bị tạm ngưng
            Terminated,     // Nhân viên đã bị chấm dứt hợp đồng
            Probation,      // Nhân viên đang trong thời gian thử việc
            Retired,         // Nhân viên đã nghỉ hưu
            ChangePasswordRequest
        }
        public enum ShiperStatus
        {
            Waiting,
            Working,

        }
        public enum CustomerStatus
        {
            Active,         // Khách hàng đang hoạt động
            Inactive,       // Khách hàng không hoạt động
            New,
            Suspended,      // Khách hàng bị tạm ngưng
            ChangePasswordRequest
        }

    }
}
