﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ApplicationCore.Entities;
using Infrastructure.Data;
using Backoffice.ViewModels;
using AutoMapper;
using Backoffice.Interfaces;
using Microsoft.Extensions.Options;
using ApplicationCore;
using Microsoft.AspNetCore.Http;

namespace Backoffice.Pages.ShopConfig
{
    public class EditModel : PageModel
    {
        private readonly DamaContext _context;
        private readonly IMapper _mapper;
        private readonly BackofficeSettings _backofficeSettings;
        private readonly IBackofficeService _service;

        public EditModel(DamaContext context, IMapper mapper, IOptions<BackofficeSettings> settings, IBackofficeService service)
        {
            _context = context;
            _mapper = mapper;
            _backofficeSettings = settings.Value;
            _service = service;
        }

        [BindProperty]
        public ShopConfigDetailViewModel ShopConfigDetailModel { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var detail = await _context.ShopConfigDetails
                .SingleOrDefaultAsync(m => m.Id == id);

            if (detail == null)
            {
                return NotFound();
            }

            ShopConfigDetailModel = _mapper.Map<ShopConfigDetailViewModel>(detail);

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            if (ShopConfigDetailModel.Picture?.Length > 0 &&  IsImageSizeInvalid(ShopConfigDetailModel.Picture))
                return Page();

            if (ShopConfigDetailModel.PictureWebp?.Length > 0 && IsImageSizeInvalid(ShopConfigDetailModel.PictureWebp))
                return Page();

            if (ShopConfigDetailModel.Picture?.Length > 0)
            {
                ShopConfigDetailModel.PictureUri = SaveFile(ShopConfigDetailModel.Picture, true, false, 1600);
                ShopConfigDetailModel.PictureMobileUri = SaveFile(ShopConfigDetailModel.Picture, true, true, 525);
            }

            if (ShopConfigDetailModel.PictureWebp != null && ShopConfigDetailModel.PictureWebp.Length > 0)
                ShopConfigDetailModel.PictureWebpUri = SaveFile(ShopConfigDetailModel.PictureWebp, false, false);

            var shopConfigDetail = _mapper.Map<ShopConfigDetail>(ShopConfigDetailModel);
            _context.Attach(shopConfigDetail).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {

            }

            return RedirectToPage("./Index");
        }

        private string SaveFile(IFormFile picture, bool resize, bool isMobile, int width = 0, int height = 0)
        {
            string addToFileName = isMobile ? "Mobile" + ShopConfigDetailModel.Id.ToString() : ShopConfigDetailModel.Id.ToString();
            return _service.SaveFile(picture, _backofficeSettings.WebNewsPictureFullPath, _backofficeSettings.WebNewsPictureUri, addToFileName, resize, width, height).PictureUri;
        }

        private bool IsImageSizeInvalid(IFormFile file)
        {
            if (file != null && file.Length > 4000000)
            {
                ModelState.AddModelError("", "A menina quer por favor diminuir o tamanho do ficheiro? O máximo é 4MB, obrigado! Ass.: O seu amor!");
                return true;
            }
            return false;
        }
    }
}
