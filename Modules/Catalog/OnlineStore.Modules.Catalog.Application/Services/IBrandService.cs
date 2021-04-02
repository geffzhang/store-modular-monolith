using System.Threading.Tasks;

namespace OnlineStore.Modules.Catalog.Application.Services
{
    public interface IBrandService
    {
        Task Create(Brand brand);

        Task Update(Brand brand);

        Task Delete(long id);

        Task Delete(Brand brand);
    }
}