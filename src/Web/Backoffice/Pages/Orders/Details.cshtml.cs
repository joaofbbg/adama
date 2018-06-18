﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApplicationCore.Entities.OrderAggregate;
using ApplicationCore.Interfaces;
using Backoffice.Extensions;
using Backoffice.Interfaces;
using Backoffice.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Backoffice.Pages.Orders
{
    public class DetailsModel : PageModel
    {
        private readonly IBackofficeService _service;
        private readonly IOrderService _orderService;
        private readonly IATService _atService;

        public DetailsModel(IBackofficeService service, IOrderService orderService, IATService atService)
        {
            _service = service;
            _orderService = orderService;
            _atService = atService;
        }

        [BindProperty]
        public OrderViewModel OrderModel { get; set; }

        [TempData]
        public string StatusMessage { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            OrderModel = await _service.GetOrder(id);
            if (OrderModel == null)
                return NotFound();
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if(ModelState.IsValid)
            {
                await _orderService.UpdateOrderState(OrderModel.Id, OrderModel.OrderState);
                StatusMessage = $"O estado da encomenda #{OrderModel.Id} foi alterada para {EnumHelper<OrderStateType>.GetDisplayValue(OrderModel.OrderState)}";
                return RedirectToPage(new { id = OrderModel.Id });                
            }
            return Page();
        }

        public async Task<IActionResult> OnPostRegisterInvoiceAsync()
        {
            try
            {
                await _atService.CreateInvoiceAsync();
            }
            catch (Exception ex)
            {
                StatusMessage = $"{ex.Message} ( {ex.InnerException?.Message})";
                return RedirectToPage(new { id = OrderModel.Id });
            }
            StatusMessage = "Sucess!!!";
            return RedirectToPage(new { id = OrderModel.Id });
        }
    }
}