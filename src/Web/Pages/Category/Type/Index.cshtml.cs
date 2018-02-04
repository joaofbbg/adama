﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Web.Interfaces;
using Web.ViewModels;

namespace Web.Pages.Category.Type
{
    public class IndexModel : PageModel
    {
        private readonly IShopService _shopService;
        private readonly ICatalogService _catalogService;
        public IndexModel(IShopService service, ICatalogService catalogService)
        {
            _shopService = service;
            _catalogService = catalogService;
        }
        [TempData]
        public string CategoryName { get; set; }
        [TempData]
        public string CatalogTypeName { get; set; }
        //public CategoryViewModel CategoryModel { get; set; } = new CategoryViewModel();
        public CatalogIndexViewModel CatalogModel { get; set; } = new CatalogIndexViewModel();

        public async Task<IActionResult> OnGetAsync(string id)
        {
            var catalogType = await _shopService.GetCatalogType(id);
            if (catalogType == null)
                return NotFound();
            CatalogTypeName = catalogType.Description;

            //int? typeId = null;
            //if (!string.IsNullOrEmpty(type))
            //{
            //    var catalogType = await _shopService.GetCatalogType(type);
            //    if (catalogType == null)
            //        return NotFound();
            //    typeId = catalogType.Id;
            //    CatalogTypeName = catalogType.Description;
            //}

            CatalogModel = await _catalogService.GetCatalogItems(0, null, null, catalogType.Id);
            //CategoryModel.CatalogTypes = CategoryModel.CatalogModel.CatalogItems.Select(x => (x.CatalogTypeCode, x.CatalogTypeName)).Distinct().ToList();

            return Page();
        }
    }
}