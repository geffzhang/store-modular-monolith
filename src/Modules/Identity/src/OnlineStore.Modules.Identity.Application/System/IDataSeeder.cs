using System.Threading.Tasks;

namespace OnlineStore.Modules.Identity.Application.System
{
    public interface IDataSeeder
    {
        Task SeedAllAsync();
    }
}