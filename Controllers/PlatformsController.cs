using System;
using Microsoft.AspNetCore.Mvc;

namespace CommandsService
{
    [Route("api/c/[controller]")]
    [ApiController]
    public class PlatformsController : ControllerBase
    {
        public PlatformsController()
        {

        }
        [HttpPost]
        public ActionResult TestInboundConnection()
        {
            Console.WriteLine("--> Inbound post CommandsService");
            return Ok("Ok Inbound post CommandsService");
        }
    }
}