﻿using ApplicationCore.Entities;

namespace Dama.API.ViewModels
{
    public class BasketItemAttributeViewModel
    {
        public int Id { get; set; }
        public CatalogAttributeType Type { get; set; }
        public string Name { get; set; }
        public decimal? Price { get; set; }
    }
}