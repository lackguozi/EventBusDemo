using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace TestApiDemo.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class ServiceController : ControllerBase
    {
        [HttpGet]
        public ActionResult<string> Test1()
        {
            return new JsonResult("helloWorld");
        }
    }
}
