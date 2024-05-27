﻿using Azure.Core;
using DiamondLuxurySolution.Data.EF;
using DiamondLuxurySolution.Data.Entities;
using DiamondLuxurySolution.ViewModel.Common;
using DiamondLuxurySolution.ViewModel.Models.About;
using DiamondLuxurySolution.ViewModel.Models.Collection;
using DiamondLuxurySolution.ViewModel.Models.Contact;
using Microsoft.EntityFrameworkCore;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondLuxurySolution.Application.Repository.Contact
{
    public class ContactRepo : IContactRepo
    {
        private readonly LuxuryDiamondShopContext _context;
        public ContactRepo(LuxuryDiamondShopContext context)
        {
            _context = context;
        }
        public async Task<ApiResult<bool>> CreateContact(CreateContactRequest request)
        {
            List<string> errorList = new List<string>();
            if (string.IsNullOrWhiteSpace(request.ContactEmailUser))
            {
                errorList.Add("Vui lòng nhập email");
            }
            if (string.IsNullOrWhiteSpace(request.ContactNameUser))
            {
                errorList.Add("Vui lòng nhập tên");
            }
            if (string.IsNullOrWhiteSpace(request.ContactPhoneUser))
            {
                errorList.Add("Vui lòng nhập số điện thoại");
            }
            if (string.IsNullOrWhiteSpace(request.content))
            {
                errorList.Add("Vui lòng nhập nội dung");
            }
            if(errorList.Any())
            {
                return new ApiErrorResult<bool>("Không hợp lệ", errorList);
            }
            var contact = new DiamondLuxurySolution.Data.Entities.Contact
            {
                content = request.content.Trim(),
                ContactPhoneUser = request.ContactPhoneUser.Trim(),
                ContactNameUser = request.ContactNameUser.Trim(),
                ContactEmailUser = request.ContactEmailUser.Trim(),                
            };

            _context.Contacts.Add(contact);
            await _context.SaveChangesAsync();
            return new ApiSuccessResult<bool>(true, "Success");
        }

        public async Task<ApiResult<bool>> DeleteContact(DeleteContactRequest request)
        {
            var contact = await _context.Contacts.FindAsync(request.ContactId);
            if (contact == null)
            {
                return new ApiErrorResult<bool>("Không tìm thấy liên hệ");
            }

            _context.Contacts.Remove(contact);
            await _context.SaveChangesAsync();
            return new ApiSuccessResult<bool>(false, "Success");
        }

        public async Task<ApiResult<ContactVm>> GetContactById(int ContactId)
        {
            var contact = await _context.Contacts.FindAsync(ContactId);
    
            if (contact == null)
            {
                return new ApiErrorResult<ContactVm>("Không tìm thấy liên hệ");
            }
            var contactVm = new ContactVm()
            {
                ContactId = contact.ContactId,
                content = contact.content.Trim(),
                ContactPhoneUser = contact.ContactPhoneUser.Trim(),
                ContactNameUser = contact.ContactNameUser.Trim(),
                ContactEmailUser = contact.ContactEmailUser.Trim(),
            };
            return new ApiSuccessResult<ContactVm>(contactVm, "Success");
        }

        public async Task<ApiResult<bool>> UpdateContact(UpdateContactRequest request)
        {
            List<string> errorList = new List<string>();
            if (string.IsNullOrWhiteSpace(request.ContactEmailUser))
            {
                errorList.Add("Vui lòng nhập email");
            }
            if (string.IsNullOrWhiteSpace(request.ContactNameUser))
            {
                errorList.Add("Vui lòng nhập tên");
            }
            if (string.IsNullOrWhiteSpace(request.ContactPhoneUser))
            {
                errorList.Add("Vui lòng nhập số điện thoại");
            }
            if (string.IsNullOrWhiteSpace(request.content))
            {
                errorList.Add("Vui lòng nhập nội dung");
            }
            if (errorList.Any())
            {
                return new ApiErrorResult<bool>("Không hợp lệ");
            }
            var contact = await _context.Contacts.FindAsync(request.ContactId);
            if (contact == null)
            {
                return new ApiErrorResult<bool>("Không tìm thấy liên hệ");
            }
            if (errorList.Any())
            {
                return new ApiErrorResult<bool>("Không hợp lệ", errorList);
            }
            contact.ContactEmailUser = request.ContactEmailUser;
            contact.ContactPhoneUser = request.ContactPhoneUser;
            contact.content = request.content;
            contact.ContactNameUser = request.ContactNameUser;

            await _context.SaveChangesAsync();
            return new ApiSuccessResult<bool>(true, "Success");
        }


        public async Task<ApiResult<PageResult<ContactVm>>> ViewContactInPaging(ViewContactRequest request)
        {
            var listContact = await _context.Contacts.ToListAsync();
            if (request.Keyword != null)
            {
                listContact = listContact.Where(x => x.ContactPhoneUser.Contains(request.Keyword) 
                || x.ContactNameUser.Contains(request.Keyword)).ToList();

            }
            listContact = listContact.OrderByDescending(x => x.ContactNameUser).ToList();

            int pageIndex = request.pageIndex ?? 1;

            var listPaging = listContact.ToPagedList(pageIndex, DiamondLuxurySolution.Utilities.Constants.Systemconstant.AppSettings.PAGE_SIZE).ToList();

            var listContactVm = listPaging.Select(contact => new ContactVm()
            {
                ContactId = contact.ContactId,
                content = contact.content.Trim(),
                ContactPhoneUser = contact.ContactPhoneUser.Trim(),
                ContactNameUser = contact.ContactNameUser.Trim(),
                ContactEmailUser = contact.ContactEmailUser.Trim(),
            }).ToList();
            var listResult = new PageResult<ContactVm>()
            {
                Items = listContactVm,
                PageSize = DiamondLuxurySolution.Utilities.Constants.Systemconstant.AppSettings.PAGE_SIZE,
                TotalRecords = listContact.Count,
                PageIndex = pageIndex
            };
            return new ApiSuccessResult<PageResult<ContactVm>>(listResult, "Success");
        }
    }
}
