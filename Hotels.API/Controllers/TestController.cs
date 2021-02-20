using System.Net.Mime;
using System;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace Hotels.API.Controllers
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    [ApiVersion("1.0", Deprecated = true)]
    [ApiVersion("1.1")]
    [ApiVersion("2.0")]
    public class TestController : ControllerBase
    {

        [HttpGet(Name = nameof(GetCustomers))]
        public IActionResult GetCustomers()
        {
            List<string>  customers = new List<string>()
            {
                "Burak Karadağ",
                "Ece Karadağ" 
            };

            return Ok(customers);
        }

        [ApiVersion("1.0", Deprecated = true)]
        [MapToApiVersion("1.1")]
        [HttpGet(Name = nameof(GetCustomerV2))]
        public IActionResult GetCustomerV2()
        {
            List<string> customers = new List<string>()
            {
                "Burak",
                "Ece"
            };

            return Ok(customers);
        }


    }
}
