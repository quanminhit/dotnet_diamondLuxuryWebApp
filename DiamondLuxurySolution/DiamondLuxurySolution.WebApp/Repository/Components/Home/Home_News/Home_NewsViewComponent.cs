﻿using DiamondLuxurySolution.ViewModel.Models.KnowledgeNews;
using DiamondLuxurySolution.WebApp.Service.KnowledgeNews;
using DiamondLuxurySolution.WebApp.Service.News;
using DiamondLuxurySolution.WebApp.Service.Product;
using DiamondLuxurySolution.WebApp.Service.Slide;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;

namespace DiamondLuxurySolution.WebApp.Repository.Components.Home_News
{
    public class Home_NewsViewComponent : ViewComponent
    {
        private readonly INewsApiService _newsApiService;

        public Home_NewsViewComponent(INewsApiService newsApiService)
        {
            _newsApiService = newsApiService;
        }
        public async Task<IViewComponentResult> InvokeAsync()
        {
            var newsList = await _newsApiService.GetAll();
            var status = newsList.ResultObj.Where(n => n.Status == true).OrderByDescending(n => n.DateCreated).Take(6).ToList();
            return View(status);
        }
    }
}
