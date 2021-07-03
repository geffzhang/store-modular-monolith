using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace OnlineStore.Modules.Identity.Application.Features.System
{
    public interface IDataSeeder
    {
        Task SeedAllAsync();
    }
}