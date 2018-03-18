﻿using ApplicationCore.Entities;
using System.Linq;

namespace ApplicationCore.Specifications
{

    public class CatalogFilterSpecification : BaseSpecification<CatalogItem>
    {
        public CatalogFilterSpecification(int? IllustrationId, int? typeId, int? categoryId)
            : base(i => i.ShowOnShop && (!IllustrationId.HasValue || i.CatalogIllustrationId == IllustrationId) &&
                (!typeId.HasValue || i.CatalogTypeId == typeId) && (!categoryId.HasValue || i.CatalogCategories.Any(x => x.CategoryId == categoryId)))
        {
            AddInclude(x => x.CatalogType);
            AddInclude(x => x.CatalogCategories);
        }
    }

    public class CatalogAttrFilterSpecification : BaseSpecification<CatalogItem>
    {
        public CatalogAttrFilterSpecification(int catalogId): base(i => i.Id == catalogId)
        {
            AddInclude(x => x.CatalogAttributes);
        }
    }

    public class CatalogTypeFilterSpecification : BaseSpecification<CatalogItem>
    {
        public CatalogTypeFilterSpecification(int catalogId) : base(i => i.Id == catalogId)
        {
            AddInclude(x => x.CatalogType);
        }
    }
}
