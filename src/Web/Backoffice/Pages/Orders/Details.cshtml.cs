﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ApplicationCore;
using ApplicationCore.DTOs;
using ApplicationCore.Entities.OrderAggregate;
using ApplicationCore.Interfaces;
using Backoffice.Extensions;
using Backoffice.Interfaces;
using Backoffice.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Options;

namespace Backoffice.Pages.Orders
{
    public class DetailsModel : PageModel
    {        
        private readonly IBackofficeService _service;
        private readonly IOrderService _orderService;
        private readonly BackofficeSettings _settings;
        private readonly IEmailSender _emailSender;

        public DetailsModel(IBackofficeService service, IOrderService orderService, IOptions<BackofficeSettings> options, IEmailSender emailSender)
        {
            _service = service;
            _orderService = orderService;
            _settings = options.Value;
            _emailSender = emailSender;
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
            SageResponseDTO response = await _service.RegisterInvoiceAsync(OrderModel.Id, OrderModel.PaymentTypeSelected);
            if (response.Message == "Success")
                StatusMessage = $"Sucesso foi criado a fatura: {response.InvoiceNumber}";
            else
                StatusMessage = $"Erro: {response.Message}, Resposta: {response.ResponseBody}";
            return RedirectToPage(new { id = OrderModel.Id });
        }

        // public async Task<IActionResult> OnPostCreatePaymentAsync()
        // {
        //     SageResponseDTO response = await _service.RegisterPaymentAsync(OrderModel.Id, OrderModel.PaymentTypeSelected);
        //     if (response.Message == "Success")
        //         StatusMessage = $"Sucesso foi criado o recibo sobre a fatura {OrderModel.SalesInvoiceNumber}";
        //     else
        //         StatusMessage = $"Erro: {response.Message}, Resposta: {response.ResponseBody}";
        //     return RedirectToPage(new { id = OrderModel.Id });
        // }

        public async Task<IActionResult> OnGetInvoicePDFAsync(int id, long invoiceId)
        {
            var fileName = string.Format(_settings.InvoiceNameFormat,id);
            //Check if file already exist
            if (_service.CheckIfFileExists(_settings.InvoicesFolderFullPath, fileName))
            {
                byte[] fileBytes = await System.IO.File.ReadAllBytesAsync(
                    Path.Combine(
                        _settings.InvoicesFolderFullPath,
                        fileName));
                return File(fileBytes, "application/pdf");
            }
            else
            {
                var bytes = await _service.GetInvoicePDF(invoiceId);

                if(bytes.Length > 0)
                    await _service.SaveFileAsync(bytes, _settings.InvoicesFolderFullPath, fileName);

                return File(bytes, "application/pdf",fileName);
            }

        }

        public async Task<IActionResult> OnGetReceiptPDFAsync(int id, long? invoiceId, long? paymentId)
        {
            if(!invoiceId.HasValue || !paymentId.HasValue)
            {
                return NotFound();
            }
            var fileName = string.Format(_settings.ReceiptNameFormat, id);
           //Check if file already exist
           if (_service.CheckIfFileExists(_settings.InvoicesFolderFullPath, fileName))
           {
                byte[] fileBytes = await System.IO.File.ReadAllBytesAsync(
                   Path.Combine(
                       _settings.InvoicesFolderFullPath,
                       fileName));
                return File(fileBytes, "application/pdf");
           }
           else
           {
               var bytes = await _service.GetReceiptPDF(invoiceId.Value,paymentId.Value);

                if(bytes.Length > 0)
                    await _service.SaveFileAsync(bytes, _settings.InvoicesFolderFullPath, fileName);

               return File(bytes, "application/pdf", fileName);
           }
        }

        public async Task<ActionResult> OnPostSendEmailToClient()
        {
            var order = await _service.GetOrder(OrderModel.Id);
            if (order == null)
                return NotFound();

            var name = order.User != null ? $"{order.User.FirstName} {order.User.LastName}" : order.BuyerId;
            var body = $"Olá {name}!<br>" +
                $"Recebemos o pagamento relativo à encomenda #{order.Id}.<br>" +
                $"Enviamos, em anexo, a fatura e o recibo relativo à encomenda.";

            var files = await _service.GetOrderDocumentsAsync(order.Id);

            await _emailSender.SendGenericEmailAsync(order.BuyerId, $"Faturação DamaNoJornal - Encomenda #{order.Id}", body, _settings.ToEmails, files);
            StatusMessage = "Mensagem Enviada";
            return RedirectToPage(new { id = OrderModel.Id });
        }
    }
}