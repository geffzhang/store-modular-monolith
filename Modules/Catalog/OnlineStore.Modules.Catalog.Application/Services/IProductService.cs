using System.Threading.Tasks;

namespace OnlineStore.Modules.Catalog.Application.Services
{
    public interface IProductService
    {
        void Create(Product product);

        void Update(Product product);

        Task Delete(Product product);
    }
}