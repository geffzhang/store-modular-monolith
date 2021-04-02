using System.Collections.Generic;
using System.Threading.Tasks;

namespace OnlineStore.Modules.Catalog.Application.Services
{
    public interface ICategoryService
    {
        Task<IList<CategoryListItem>> GetAll();

        Task Create(Category category);

        Task Update(Category category);

        Task Delete(Category category);
    }
}