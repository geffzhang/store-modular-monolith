using System.Threading;
using System.Threading.Tasks;
using OnlineStore.Modules.Catalog.Domain.Entities;

namespace OnlineStore.Modules.Catalog.Domain.Events
{
    public class ReviewSummaryChangedHandler : INotificationHandler<ReviewSummaryChanged>
    {
        private readonly IRepository<Product> _productRepository;

        public ReviewSummaryChangedHandler(IRepository<Product> productRepository)
        {
            _productRepository = productRepository;
        }

        public async Task Handle(ReviewSummaryChanged notification, CancellationToken cancellationToken)
        {
            if (notification.EntityTypeId == "Product")
            {
                var product = await _productRepository.Query().FirstAsync(x => x.Id == notification.EntityId);
                product.ReviewsCount = notification.ReviewsCount;
                product.RatingAverage = notification.RatingAverage;
            }
        }
    }
}