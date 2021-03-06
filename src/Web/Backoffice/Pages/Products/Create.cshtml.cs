﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using ApplicationCore.Entities;
using Infrastructure.Data;
using AutoMapper;
using Backoffice.ViewModels;
using Microsoft.EntityFrameworkCore;
using Backoffice.Interfaces;
using Microsoft.Extensions.Options;
using Backoffice.Extensions;
using ApplicationCore;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace Backoffice.Pages.Products
{
    public class CreateModel : PageModel
    {
        private readonly DamaContext _context;
        private readonly IMapper _mapper;
        private readonly IBackofficeService _service;
        private readonly BackofficeSettings _backofficeSettings;

        public CreateModel(DamaContext context, IMapper mapper, IBackofficeService service, IOptions<BackofficeSettings> backofficeSettings)
        {
            _context = context;
            _mapper = mapper;
            _service = service;
            _backofficeSettings = backofficeSettings.Value;
        }
        [BindProperty]
        public ProductCreateViewModel ProductModel { get; set; }

        [BindProperty]
        public List<CatalogCategoryViewModel> CatalogCategoryModel { get; set; } = new List<CatalogCategoryViewModel>();

        public class ProductCreateViewModel
        {
            public int Id { get; set; }
            [Required]
            [StringLength(100)]
            [Display(Name = "Nome")]
            public string Name { get; set; }
            [Required]
            [StringLength(100)]
            public string Slug { get; set; }

            [Display(Name = "Descrição")]
            public string Description { get; set; }
            [Display(Name = "Preço")]
            public decimal? Price { get; set; }
            [Display(Name = "Ilustração")]
            [Required]
            public int CatalogIllustrationId { get; set; }
            [Required]
            [Display(Name = "Tipo de Produto")]
            public int CatalogTypeId { get; set; }
            [Display(Name = "SKU")]
            public string Sku { get; set; }
            [Display(Name = "Loja")]
            public bool ShowOnShop { get; set; }
            [Display(Name = "Novidade")]
            public bool IsNew { get; set; }
            [Display(Name = "Destaque")]
            public bool IsFeatured { get; set; }
            [Display(Name = "Personalizar")]
            public bool CanCustomize { get; set; }
            [Display(Name = "Indisponível")]
            public bool IsUnavailable { get; set; }
            [Display(Name = "Imagem Principal")]
            public IFormFile Picture { get; set; }
            [Display(Name = "URL da Imagem Principal")]
            public string PictureUri { get; set; }
            [Display(Name = "Imagens do Produto")]
            public List<IFormFile> OtherPictures { get; set; }
            public int Stock { get; set; }
            [Display(Name = "Meta Description")]
            [StringLength(160)]
            public string MetaDescription { get; set; }
            [Display(Name = "Title")]
            [StringLength(43)]
            public string Title { get; set; }
            [Display(Name = "Desconto")]
            public decimal? Discount { get; set; }


            public IList<ProductAttributeViewModel> Attributes { get; set; } = new List<ProductAttributeViewModel>();
            [Display(Name = "Imagens do Produto")]
            public IList<ProductPictureViewModel> Pictures { get; set; } = new List<ProductPictureViewModel>();
            [Display(Name = "Categorias")]
            public IList<CatalogCategoryViewModel> Categories { get; set; } = new List<CatalogCategoryViewModel>();
            public IList<CatalogReference> References { get; set; } = new List<CatalogReference>();

        }

        public async Task<IActionResult> OnGet()
        {
            await PopulateLists();
            return Page();
        }        


        public async Task<IActionResult> OnPostSaveAsync()
        {
            if (!ModelState.IsValid)
            {
                await PopulateLists();
                return Page();
            }

            if (!ValidatePictures())
            {
                await PopulateLists();
                return Page();
            }
            //Fix Slug
            ProductModel.Slug = Utils.URLFriendly(ProductModel.Slug);

            if((await SlugExistsAsync(ProductModel.Slug)))
            {
                ModelState.AddModelError("ProductModel.Slug", "Já existe um slug com o mesmo nome!");
                await PopulateLists();
                return Page();
            }

            ProductModel.Sku = await _service.GetSku(ProductModel.CatalogTypeId, ProductModel.CatalogIllustrationId);

            
            ProductModel.Price = ProductModel.Price == 0 ? default(decimal?) : ProductModel.Price;
            
            var prod = _mapper.Map<CatalogItem>(ProductModel);            

            //Save Main Image            
            if (ProductModel.Picture?.Length > 0)
            {
                var lastCatalogItemId = 0;
                if (_context.CatalogItems.Any())
                    lastCatalogItemId = GetLastCatalogId();

                var info = _service.SaveFile(ProductModel.Picture, _backofficeSettings.WebProductsPictureV2FullPath, _backofficeSettings.WebProductsPictureV2Uri, (++lastCatalogItemId).ToString(), true, 700, 700);
                prod.UpdateMainPicture(info.PictureUri);

                prod.AddPicture(new CatalogPicture(true, true, info.PictureUri,0, info.PictureHighUri));
            }

            //Save other images
            if (ProductModel.OtherPictures?.Count > 0)
            {
                var lastCatalogPictureId = _context.CatalogPictures.Count() > 0 ? GetLastCatalogPictureId() : 0;
                var order = 0;
                foreach (var item in ProductModel.OtherPictures)
                {
                    var info = _service.SaveFile(item, _backofficeSettings.WebProductsPictureV2FullPath, _backofficeSettings.WebProductsPictureV2Uri, (++lastCatalogPictureId).ToString(), true, 700, 700);
                    prod.AddPicture(new CatalogPicture(true,false, info.PictureUri,++order, info.PictureHighUri));
                }
            }

            //Categories
            foreach (var item in CatalogCategoryModel.Where(x => x.Selected).ToList())
            {
                prod.AddCategory(item.CategoryId);
                foreach (var child in item.Childs.Where(x => x.Selected).ToList())
                {
                    prod.AddCategory(child.CategoryId);
                }
            }

            //Save Changes                        
            _context.CatalogItems.Add(prod);
            await _context.SaveChangesAsync();

            //Update Sku
            prod.UpdateSku(prod.Sku + "_" + prod.Id);
            await _context.SaveChangesAsync();
            
            return RedirectToPage("./Index");
        }

        public async Task<IActionResult> OnPostAddAttributeAsync()
        {
            await PopulateLists();
            ProductModel.Attributes.Add(new ProductAttributeViewModel());
            return Page();
        }

        public async Task<IActionResult> OnGetCategoriesAsync(int productType)
        {
            var cats = await _service.GetCategoriesAsync(productType);
            return new JsonResult(cats.Select(x => x.Id).ToList());
        }

        public async Task<IActionResult> OnGetProductTypePriceAsync(int productType)
        {
            var type = await _context.CatalogTypes.FindAsync(productType);
            return new JsonResult(type.Price);
        }

        private int GetLastCatalogId()
        {
            return _context.CatalogItems
                .AsEnumerable()
                .Last().Id;
        }

        private int GetLastCatalogPictureId()
        {
            return _context.CatalogPictures
                .AsNoTracking()
                .AsEnumerable()
                .Last()
                .Id;
        }

        private async Task<bool> SlugExistsAsync(string slug)
        {
            return await _context.CatalogItems.AnyAsync(x => x.Slug == slug);
        }

        private bool ValidatePictures()
        {
            // if (ProductModel.Picture == null || ProductModel.Picture.Length == 0)
            // {
            //     ModelState.AddModelError("", "A menina quer por favor escolher uma imagem principal, obrigado! Ass.: O seu amor!");                
            // }

            if (ProductModel.Picture?.Length > 2097152)
            {
                ModelState.AddModelError("", "A menina quer por favor diminuir o tamanho da imagem principal? O máximo é 2MB, obrigado! Ass.: O seu amor!");               
            }

            if (ProductModel.OtherPictures?.Count > 0)
            {
                foreach (var item in ProductModel.OtherPictures)
                {
                    if (item.Length > 2097152)
                        ModelState.AddModelError("", $"A imagem {item.GetFileName()} está muito grande amor, O máximo é 2MB, obrigado!");
                }
            }
            return ModelState.IsValid;
        }

        private async Task PopulateLists()
        {
            var illustrationsDb = await _context.CatalogIllustrations
               .Include(x => x.IllustrationType)
               .AsNoTracking()
               .ToListAsync();
            var illustrations = illustrationsDb
                .Select(s => new { s.Id, Name = $"{s.Code} - {s.IllustrationType.Code} - {s.Name}" })
                .OrderBy(x => x.Name)
                .ToList();
            var typesDb = await _context.CatalogTypes
                .AsNoTracking()
                .ToListAsync();
            ViewData["IllustrationId"] = new SelectList(illustrations, "Id", "Name");
            var types = typesDb
                .Select(x => new { x.Id, Name = $"{x.Code} - {x.Name}" });
            ViewData["ProductTypeId"] = new SelectList(types, "Id", "Name");
            await SetCatalogCategoryModel(types.First().Id);
        }

        private async Task SetCatalogCategoryModel(int catalogTypeId)
        {
            //Catalog Categories            
            var allCats = await _context.Categories
                .Include(x => x.Parent)
                .Include(x => x.CatalogTypes)
                .ToListAsync();

            var catsId = allCats
                .Where(x => x.CatalogTypes.Any(ct => ct.CatalogTypeId == catalogTypeId))
                .Select(x => x.Id)
                .ToList();

            foreach (var item in allCats.Where(x => x.Parent == null).ToList())
            {
                CatalogCategoryViewModel parent = new CatalogCategoryViewModel
                {
                    CategoryId = item.Id,
                    Label = item.Name,
                    Childs = new List<CatalogCategoryViewModel>(),
                    Selected = catsId.Contains(item.Id)

                };
                parent.Childs.AddRange(allCats.Where(x => x.ParentId == item.Id).Select(s => new CatalogCategoryViewModel
                {
                    CategoryId = s.Id,
                    Label = s.Name,
                    Selected = catsId.Contains(s.Id)
                }));
                CatalogCategoryModel.Add(parent);
            }
        }
    }
}
