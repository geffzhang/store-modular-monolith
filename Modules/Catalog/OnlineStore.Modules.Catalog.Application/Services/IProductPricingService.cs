using System;

namespace OnlineStore.Modules.Catalog.Application.Services
{
    public interface IProductPricingService
    {
        CalculatedProductPrice CalculateProductPrice(ProductThumbnail productThumbnail);

        CalculatedProductPrice CalculateProductPrice(Product product);

        CalculatedProductPrice CalculateProductPrice(decimal price, decimal? oldPrice, decimal? specialPrice,
            DateTimeOffset? specialPriceStart, DateTimeOffset? specialPriceEnd);
    }
}