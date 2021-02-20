using System;
using Hotels.API.Models;
using Hotels.API.Models.Derived;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Hotels.API.Controllers
{
    [Route("/[controller]")]
    [ApiController]
    public class InfoController : ControllerBase
    {
        private readonly HotelInfo _hotelInfo;

        public InfoController(IOptions<HotelInfo> hotleInfoOption)
        {
            _hotelInfo = hotleInfoOption.Value;
        }

        [HttpGet(Name = nameof(GetInfo))]
        [ProducesResponseType(200)]
        public ActionResult<HotelInfo> GetInfo()
        {
            _hotelInfo.Href = Url.Link(nameof(GetInfo),null);
            return _hotelInfo;
        }

    }
}
