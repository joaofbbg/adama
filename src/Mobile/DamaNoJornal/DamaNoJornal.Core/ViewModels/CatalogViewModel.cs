﻿using DamaNoJornal.Core.Models.Basket;
using DamaNoJornal.Core.Models.Catalog;
using DamaNoJornal.Core.Models.Navigation;
using DamaNoJornal.Core.Services.Basket;
using DamaNoJornal.Core.Services.Catalog;
using DamaNoJornal.Core.Services.Settings;
using DamaNoJornal.Core.Services.User;
using DamaNoJornal.Core.ViewModels.Base;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace DamaNoJornal.Core.ViewModels
{
    public class CatalogViewModel : ViewModelBase
    {
        private ObservableCollection<CatalogItem> _products;
        private ObservableCollection<CatalogBrand> _brands;
        private CatalogBrand _brand;
        private ObservableCollection<CatalogType> _types;
        private CatalogType _type;
        private ICatalogService _productsService;
        private ISettingsService _settingsService;
        private IBasketService _basketService;
        private IUserService _userService;

        public CatalogViewModel(
            ICatalogService productsService,
            ISettingsService settingsService,
            IBasketService basketService,
            IUserService userService)
        {
            _productsService = productsService;
            _settingsService = settingsService;
            _basketService = basketService;
            _userService = userService;
        }

        public ObservableCollection<CatalogItem> Products
        {
            get { return _products; }
            set
            {
                _products = value;
                RaisePropertyChanged(() => Products);
            }
        }

        public ObservableCollection<CatalogBrand> Brands
        {
            get { return _brands; }
            set
            {
                _brands = value;
                RaisePropertyChanged(() => Brands);
            }
        }

        public CatalogBrand Brand
        {
            get { return _brand; }
            set
            {
                _brand = value;
                RaisePropertyChanged(() => Brand);
                RaisePropertyChanged(() => IsFilter);
                RaisePropertyChanged(() => FilterText);
            }
        }

        public ObservableCollection<CatalogType> Types
        {
            get { return _types; }
            set
            {
                _types = value;
                RaisePropertyChanged(() => Types);
            }
        }

        public CatalogType Type
        {
            get { return _type; }
            set
            {
                _type = value;
                RaisePropertyChanged(() => Type);
                RaisePropertyChanged(() => IsFilter);
                RaisePropertyChanged(() => FilterText);
            }
        }

        public string FilterText
        {
            get
            {
                if (Brand != null && Type != null)
                    return $"Filtrar ({Brand.Name},{Type.Description})";
                else if (Brand != null)
                    return $"Filtrar ({Brand.Name})";
                else if (Type != null)
                    return $"Filtrar ({Type.Description})";
                else
                    return "Filtrar";
            }
            set
            {
                RaisePropertyChanged(() => FilterText);
            }
        }

        public bool IsFilter { get { return Brand != null || Type != null; } }

        public ICommand AddCatalogItemCommand => new Command<CatalogItem>(async (item) => await AddCatalogItemAsync(item));

        public ICommand FilterCommand => new Command(async () => await FilterAsync());

        public override async Task InitializeAsync(object navigationData)
        {
            IsBusy = true;

            if (navigationData is Tuple<CatalogBrand, CatalogType>)
            {
                var tuple = (Tuple<CatalogBrand, CatalogType>)navigationData;
                Brand = tuple.Item1;
                Type = tuple.Item2;
                Products = await _productsService.FilterAsync(Brand?.Id, Type?.Id);
            }
            else
            {
                Products = await _productsService.GetCatalogAsync();
            }

            // Get Catalog, Brands and Types
            //Products = await _productsService.GetCatalogAsync();
            //Brands = await _productsService.GetCatalogCategoryAsync();
            //Types = await _productsService.GetCatalogTypeAsync();

            IsBusy = false;
        }

        private async Task AddCatalogItemAsync(CatalogItem catalogItem)
        {
            //Attributes
            List<CatalogAttribute> attributesSelected = new List<CatalogAttribute>();

            //For each catalog attribute
            if (catalogItem.CatalogAttributes?.Count > 0)
            {
                var group = catalogItem.CatalogAttributes.GroupBy(x => x.Type);
                foreach (var item in group)
                {
                    var result = await DialogService.ShowPromptAsync("Atributos", "Cancelar", item.Select(x => $"{AttributeTypeHelper.GetTypeDescription(item.Key)} {x.Name}").ToArray());
                    if (!string.IsNullOrEmpty(result))
                    {
                        var attribute = item.SingleOrDefault(x => result.Contains(x.Name));
                        if (attribute != null)
                            attributesSelected.Add(attribute);
                        System.Diagnostics.Debug.WriteLine($"############## Atributo: {result} #################");
                    }
                }
            }

            //Create BasketItem
            var basketItem = new BasketItem
            {
                ProductId = catalogItem.Id,
                ProductName = catalogItem.Name,
                PictureUrl = catalogItem.PictureUri,
                UnitPrice = catalogItem.Price,
                Quantity = 1,
                Attributes = attributesSelected.Select(x => new BasketItemAttribute
                {
                    Id = x.Id,
                    Name = x.Name,
                    Price = x.Price,
                    Type = x.Type
                }).ToList()
            };

            //Call service
            var authToken = _settingsService.AuthAccessToken;
            var userInfo = await _userService.GetUserInfoAsync(authToken);

            var basket = await _basketService.AddBasketItemAsync(new CustomerBasket
            {
                BuyerId = userInfo.Email,
                Items = new List<BasketItem> { basketItem }
            }, authToken);

            // Add new item to Basket
            MessagingCenter.Send<CatalogViewModel, int>(this, MessageKeys.AddProduct, basket.Items?.Count ?? 0);
            DialogService.ShowToastMessage("Produto adicionado ao carrinho!");
            //await NavigationService.NavigateToAsync<BasketViewModel>((catalogItem, attributesSelected));
        }

        private async Task FilterAsync()
        {
            await NavigationService.NavigateToAsync<CatalogFilterViewModel>();
            //if (Brand == null || Type == null)
            //{
            //    return;
            //}

            //IsBusy = true;

            //// Filter catalog products
            //MessagingCenter.Send(this, MessageKeys.Filter);
            //Products = await _productsService.FilterAsync(Brand.Id, Type.Id);

            //IsBusy = false;
        }
    }
}