using Microsoft.AspNetCore.Mvc;

namespace OnlineStore.Modules.Identity.Api.Controllers
{
    [ApiController]
    [Route("")]
    public class HomeController : ControllerBase
    {
        [HttpGet]
        public string Index()
        {
            return "Identity Module";
        }
    }
}