﻿using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace Web.ViewModels
{
    public class CatalogIndexViewModel
    {
        public IEnumerable<CatalogItemViewModel> CatalogItems { get; set; }
        public IEnumerable<CatalogItemViewModel> NewCatalogItems { get; set; }
        public IEnumerable<CatalogItemViewModel> FeaturedCatalogItems { get; set; }
        public IEnumerable<SelectListItem> Brands { get; set; }
        public IEnumerable<SelectListItem> Types { get; set; }
        public int? BrandFilterApplied { get; set; }
        public int? TypesFilterApplied { get; set; }
        public PaginationInfoViewModel PaginationInfo { get; set; }

        //Dama Stuff
        public List<MainBannerViewModel> Banners { get; set; }
    }
}
