﻿using DiamondLuxurySolution.Data.Entities;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondLuxurySolution.Application.Repository.Role
{
	public class RoleInitializer : IRoleInitializer
	{
		private readonly RoleManager<AppRole> _roleManager;
		private readonly UserManager<AppUser> _userManager;
		public RoleInitializer(RoleManager<AppRole> roleManager, UserManager<AppUser> userManager)
		{
			_roleManager = roleManager;
			_userManager = userManager;
		}

		public async Task CreateDefaultRole()
		{
			var AdminRoleName = DiamondLuxurySolution.Utilities.Constants.Systemconstant.UserRoleDefault.Admin.ToString();
			var CustomerRoleName = DiamondLuxurySolution.Utilities.Constants.Systemconstant.UserRoleDefault.Customer.ToString();
			var DeliveryStaffRoleName = DiamondLuxurySolution.Utilities.Constants.Systemconstant.UserRoleDefault.DeliveryStaff.ToString();
			var ManagerRoleName = DiamondLuxurySolution.Utilities.Constants.Systemconstant.UserRoleDefault.Manager.ToString();
			var SalesStaffRoleName = DiamondLuxurySolution.Utilities.Constants.Systemconstant.UserRoleDefault.SalesStaff.ToString();

			var defaultRoleNameList = new List<string>();
			defaultRoleNameList.Add(AdminRoleName);
			defaultRoleNameList.Add(CustomerRoleName);
			defaultRoleNameList.Add(DeliveryStaffRoleName);
			defaultRoleNameList.Add(ManagerRoleName);
			defaultRoleNameList.Add(SalesStaffRoleName);

			foreach (var roleName in defaultRoleNameList)
			{
				var role = await _roleManager.FindByNameAsync(roleName);
				if (role != null)
				{
					continue;
				}
				var roleAdd = new AppRole()
				{
					Name = roleName,
					Description = roleName
				};
				var status = await _roleManager.CreateAsync(roleAdd);
			}
		}

		public async Task CreateAdminAccount()
		{
			var adminUserName = "Admin1@";
			if (await _userManager.FindByNameAsync(adminUserName) == null)
			{
				var admin = new AppUser()
				{
					Fullname = "Admin1@",
					Email = "Admin1@gmail.com",
					Dob = new DateTime(2003, 06, 20),
					DateCreated = DateTime.Now,
					PhoneNumber = "0999999999",
					UserName = adminUserName,
					Status = DiamondLuxurySolution.Utilities.Constants.Systemconstant.StaffStatus.Active.ToString(),
					Image = "https://firebasestorage.googleapis.com/v0/b/diamondluxuryshop-980cd.appspot.com/o/images%2Favatar-1.jpg?alt=media&token=0da5ca0b-e36d-40c3-919c-bce14697a73c"
				};

				var result = await _userManager.CreateAsync(admin, "Admin1@");
				if (result.Succeeded)
				{
					var adminRole = await _roleManager.FindByNameAsync(DiamondLuxurySolution.Utilities.Constants.Systemconstant.UserRoleDefault.Admin.ToString());
					if (adminRole != null)
					{
						await _userManager.AddToRoleAsync(admin, adminRole.Name);
					}
				}
				else
				{
					throw new Exception(string.Join(", ", result.Errors.Select(e => e.Description)));
				}
			}
		}

		public async Task CreateManagerAccount()
		{
			var managerUserName = "Manager1@";
			if (await _userManager.FindByNameAsync(managerUserName) == null)
			{
				var manager = new AppUser()
				{
					Fullname = "Manager1@",
					Email = "Manager1@gmail.com",
					DateCreated = DateTime.Now,

					Dob = new DateTime(2003, 06, 29),
					PhoneNumber = "0999999991",
					UserName = managerUserName,
					Status = DiamondLuxurySolution.Utilities.Constants.Systemconstant.StaffStatus.Active.ToString(),
					Image = "https://firebasestorage.googleapis.com/v0/b/diamondluxuryshop-980cd.appspot.com/o/images%2Favatar-2.jpg?alt=media&token=11e58ad2-18c4-44ec-a5a8-3941669442f2"
				};

				var result = await _userManager.CreateAsync(manager, "Manager1@");
				if (result.Succeeded)
				{
					var managerRole = await _roleManager.FindByNameAsync(DiamondLuxurySolution.Utilities.Constants.Systemconstant.UserRoleDefault.Manager.ToString());
					if (managerRole != null)
					{
						await _userManager.AddToRoleAsync(manager, managerRole.Name);
					}
				}
				else
				{
					throw new Exception(string.Join(", ", result.Errors.Select(e => e.Description)));
				}
			}
		}

		public async Task CreateSaleStaffAccount()
		{
			var saleUserName = "Sale1@";
			if (await _userManager.FindByNameAsync(saleUserName) == null)
			{
				var sale = new AppUser()
				{
					Fullname = "Sale1@",
					Email = "Sale1@gmail.com",
					Dob = new DateTime(2003, 04, 12),
					DateCreated = DateTime.Now,
					PhoneNumber = "0999999992",
					UserName = saleUserName,
					Status = DiamondLuxurySolution.Utilities.Constants.Systemconstant.StaffStatus.Active.ToString(),
					Image = "https://firebasestorage.googleapis.com/v0/b/diamondluxuryshop-980cd.appspot.com/o/images%2Favatar-3.jpg?alt=media&token=e9abd274-edaa-4f95-9aef-5ea400bc3f2a"
				};

				var result = await _userManager.CreateAsync(sale, "Sale1@");
				if (result.Succeeded)
				{
					var saleRole = await _roleManager.FindByNameAsync(DiamondLuxurySolution.Utilities.Constants.Systemconstant.UserRoleDefault.SalesStaff);
					if (saleRole != null)
					{
						await _userManager.AddToRoleAsync(sale, saleRole.Name);
					}
				}
				else
				{
					throw new Exception(string.Join(", ", result.Errors.Select(e => e.Description)));
				}
			}
		}

		public async Task CreateShipperAccount()
		{
			var shipperUserName = "Shipper1@";
			if (await _userManager.FindByNameAsync(shipperUserName) == null)
			{
				var shipper = new AppUser()
				{
					Fullname = "Shipper1",
					Email = "Shipper1@gmail.com",
					Dob = new DateTime(2003, 06, 21),
					DateCreated = DateTime.Now,
					PhoneNumber = "0999999993",
					UserName = shipperUserName,
					Status = DiamondLuxurySolution.Utilities.Constants.Systemconstant.StaffStatus.Active.ToString(),
					ShipStatus = DiamondLuxurySolution.Utilities.Constants.Systemconstant.ShiperStatus.Waiting.ToString(),
					Image = "https://firebasestorage.googleapis.com/v0/b/diamondluxuryshop-980cd.appspot.com/o/images%2Favatar-4.jpg?alt=media&token=0bec7c0b-b54d-42bb-a7c3-7e865a27ffeb"
				};

				var result = await _userManager.CreateAsync(shipper, "Shipper1@");
				if (result.Succeeded)
				{
					var shipperRole = await _roleManager.FindByNameAsync(DiamondLuxurySolution.Utilities.Constants.Systemconstant.UserRoleDefault.DeliveryStaff);
					if (shipperRole != null)
					{
						await _userManager.AddToRoleAsync(shipper, shipperRole.Name);
					}
				}
				else
				{
					throw new Exception(string.Join(", ", result.Errors.Select(e => e.Description)));
				}
			}
		}

		public async Task CreateCustomerAccount()
		{
			var customerUserName = "Customer1@";
			if (await _userManager.FindByNameAsync(customerUserName) == null)
			{
				var customer = new AppUser()
				{
					Fullname = "Customer1",
					Email = "Customer1@gmail.com",
					Dob = new DateTime(2003, 06, 29),
					PhoneNumber = "0999999994",
					DateCreated = DateTime.Now,
					UserName = customerUserName,
					Point = 1000,
					Status = DiamondLuxurySolution.Utilities.Constants.Systemconstant.StaffStatus.Active.ToString(),
				};

				var result = await _userManager.CreateAsync(customer, "Customer1@");
				if (result.Succeeded)
				{
					var customerRole = await _roleManager.FindByNameAsync(DiamondLuxurySolution.Utilities.Constants.Systemconstant.UserRoleDefault.Customer);
					if (customerRole != null)
					{
						await _userManager.AddToRoleAsync(customer, customerRole.Name);
					}
				}
				else
				{
					throw new Exception(string.Join(", ", result.Errors.Select(e => e.Description)));
				}
			}
		}
	}

}
