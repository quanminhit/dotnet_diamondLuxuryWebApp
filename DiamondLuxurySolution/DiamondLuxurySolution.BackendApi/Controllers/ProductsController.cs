﻿using DiamondLuxurySolution.Application.Repository.Gem;
using DiamondLuxurySolution.Application.Repository.Product;
using DiamondLuxurySolution.Data.EF;
using DiamondLuxurySolution.Data.Entities;
using DiamondLuxurySolution.ViewModel.Models;
using DiamondLuxurySolution.ViewModel.Models.Gem;
using DiamondLuxurySolution.ViewModel.Models.Product;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace DiamondLuxurySolution.BackendApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly LuxuryDiamondShopContext _context;
        private readonly IProductRepo _product;

        public ProductsController(LuxuryDiamondShopContext context, IProductRepo product)
        {
            _context = context;
            _product = product;
        }



            [HttpPost("Create")]
            public async Task<ActionResult> CreateProduct([FromForm] CreateProductRequest request)
            {
                try
                {
                    if (!string.IsNullOrEmpty(request.ListSubGemsJson))
                    {
                        request.ListSubGems = JsonConvert.DeserializeObject<List<SubGemSupportDTO>>(request.ListSubGemsJson);
                    }

                    var status = await _product.CreateProduct(request);
                    if (status.IsSuccessed)
                    {
                        return Ok(status);
                    }
                    return BadRequest(status);
                }
                catch (Exception e)
                {
                    return BadRequest(e.Message);
                }
            }


        [HttpPut("Update")]
        public async Task<ActionResult> UpdateProduct([FromForm] UpdateProductRequest request)
        {
            try
            {
                if (!string.IsNullOrEmpty(request.ListSubGemsJson))
                {
                    request.ListSubGems = JsonConvert.DeserializeObject<ICollection<SubGemSupportDTO>>(request.ListSubGemsJson);
                }
                if (!string.IsNullOrEmpty(request.ListExistingSubGemsJson))
                {
                    request.ExistingListSubGems = JsonConvert.DeserializeObject<ICollection<SubGemSupportDTO>>(request.ListExistingSubGemsJson);
                }
                var status = await _product.UpdateProduct(request);
                if (status.IsSuccessed)
                {
                    return Ok(status);
                }
                return BadRequest(status);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }


        [HttpDelete("Delete")]
        public async Task<IActionResult> DeleteProduct([FromQuery] DeleteProductRequest request)
        {
            try
            {
                var status = await _product.DeleteProduct(request);
                if (status.IsSuccessed)
                {
                    return Ok(status);
                }
                return BadRequest(status);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpGet("GetProductById")]
        public async Task<IActionResult> FindProductById([FromQuery] string ProductId)
        {
            try
            {
                var status = await _product.GetProductById(ProductId);
                if (status.IsSuccessed)
                {
                    return Ok(status);
                }
                return BadRequest(status);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }


        [HttpGet("ViewProduct")]
        public async Task<IActionResult> ViewAllProductPagination([FromQuery] ViewProductRequest request)
        {
            try
            {
                var status = await _product.ViewProduct(request);
                if (status.IsSuccessed)
                {
                    return Ok(status);
                }
                return BadRequest(status);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

		[HttpGet("GetAll")]
		public async Task<IActionResult> GetAll()
		{
			try
			{
				var status = await _product.GetAll();
				if (status.IsSuccessed)
				{
					return Ok(status);
				}
				return BadRequest(status);
			}
			catch (Exception e)
			{
				return BadRequest(e.Message);
			}
		}


        [HttpGet("ViewProductCategorySale")]
        public async Task<IActionResult> ViewProductCategorySale()
        {
            try
            {
                var status = await _product.ViewProductCategorySale();
                if (status.IsSuccessed)
                {
                    return Ok(status);
                }
                return BadRequest(status);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpGet("ViewProductSale12Days")]
        public async Task<IActionResult> ViewProductSale12Days()
        {
            try
            {
                var status = await _product.ViewProductSale12Days();
                if (status.IsSuccessed)
                {
                    return Ok(status);
                }
                return BadRequest(status);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}
