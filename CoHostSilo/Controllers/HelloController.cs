using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GrainInterfaces;
using Orleans;

namespace CoHostSilo.Controllers
{
    [ApiController]
    [Route("api/hello")]
    public class HelloController : ControllerBase
    {
        private readonly IGrainFactory _client;
        private readonly IHello _grain;

        public HelloController(IGrainFactory client)
        {
            _client = client;
            _grain = _client.GetGrain<IHello>(0);
        }

        [HttpGet]
        public Task<OutgoingMessage> SayHello() => this._grain.SayHello(new IncomeMessage{ msg ="a"});
    }
}
