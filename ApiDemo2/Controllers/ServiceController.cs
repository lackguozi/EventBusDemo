using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ApiDemo2.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class Service2Controller : ControllerBase
    {
        [HttpGet]
        public ActionResult<string> Test2()
        {
            return new JsonResult("helloWorld");
        }
    }
}
