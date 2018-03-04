﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApplicationCore.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Web.Interfaces;
using Web.ViewModels;

namespace Web.Pages.Product
{
    public class IndexModel : PageModel
    {
        private readonly ICatalogService _catalogService;
        private readonly IAppLogger<IndexModel> _logger;
        public IndexModel(ICatalogService catalogService, IAppLogger<IndexModel> logger)
        {
            _catalogService = catalogService;
            _logger = logger;
        }
        public ProductViewModel ProductModel { get; set; } = new ProductViewModel();

        public async Task<IActionResult> OnGetAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
                return NotFound();
            ProductModel = await _catalogService.GetCatalogItem(id);

            //Update default price
            decimal attrDefaultPrice = 0M;
            foreach (var item in ProductModel.Attributes.GroupBy(x => x.AttributeType))
            {                
                attrDefaultPrice += item.First().Attributes.First().Price ?? 0;
            }
            _logger.LogInformation(">>>>>>> Product Base Price: {0} <<<<<<<<<<<", ProductModel.ProductBasePrice);
            _logger.LogInformation(">>>>>>> Atribute Default Price: {0} <<<<<<<<<<<", attrDefaultPrice);
            ProductModel.ProductTotalPrice = ProductModel.ProductBasePrice + attrDefaultPrice;
            return Page();
        }

        public async Task<JsonResult> OnGetAttributeDetailsAsync(int id)
        {
            var res = await _catalogService.GetAttributeDetails(id);
            return new JsonResult(new { price = res.Price, sku = res.ReferenceCatalogSku });
        }
    }
}