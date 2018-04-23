﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Web.ViewModels;

namespace Web.Interfaces
{
    public interface ICustomizeViewModelService
    {
        Task<CustomizeViewModel> GetCustomizeItems(int? categoryId, int? catalogItemId);
        Task SendCustomizeService(CustomizeViewModel request);
    }
}
