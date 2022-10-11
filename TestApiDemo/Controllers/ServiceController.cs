using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace TestApiDemo.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class ServiceController : ControllerBase
    {
        private readonly IOptionsMonitor<MsgOption> msg;

        public ServiceController(IOptionsMonitor<MsgOption> msg)
        {
            this.msg = msg;
        }

        [HttpGet]
        public ActionResult<string> Test1()
        {
            return new JsonResult("helloWorld");
        }
        [HttpGet]
        public Task Run()
        {
            Console.WriteLine(msg.CurrentValue.Name);
            Console.WriteLine(msg.CurrentValue.Value);
            return Task.CompletedTask;
        }
    }
    
}
