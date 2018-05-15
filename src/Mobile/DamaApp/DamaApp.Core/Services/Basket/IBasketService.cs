﻿using DamaApp.Core.Models.Basket;
using System.Threading.Tasks;

namespace DamaApp.Core.Services.Basket
{
    public interface IBasketService
    {
        Task<CustomerBasket> GetBasketAsync(string guidUser, string token);
        Task<CustomerBasket> UpdateBasketAsync(CustomerBasket customerBasket, string token);
        Task CheckoutAsync(BasketCheckout basketCheckout, string token);
        Task ClearBasketAsync(string guidUser, string token);
    }
}