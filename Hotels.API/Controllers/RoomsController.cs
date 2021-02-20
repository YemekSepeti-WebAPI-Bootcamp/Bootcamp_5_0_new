using System.Data;
using System.Net;
using System.Data.Common;
using System;
using Hotels.API.Contexts;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using Hotels.API.Services;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Primitives;
using Hotels.API.Models.Filters;
using Microsoft.AspNetCore.Http;
using Hotels.API.Extensions;

namespace Hotels.API.Controllers
{
    [Authorize]
    [Route("/[controller]/[action]")]
    [ApiController]
    public class RoomsController : ControllerBase
    {

        private readonly IRoomService _roomService;
        public RoomsController(IRoomService roomService)
        {
            _roomService = roomService;
        }

        [HttpGet(Name = nameof(GetRooms))]
        public async Task<IActionResult> GetRooms()
        {
            var rooms = await _roomService.GetRoomsAsync();

            if (rooms == null)
                return NoContent();

            return Ok(rooms);

        }


        #region SimpleFiltering
        [HttpGet(Name = nameof(GetRoomsSimpleFilter))]
        public async Task<IActionResult> GetRoomsSimpleFilter()
        {
            StringValues nameFilter;
            StringValues rateFilter;

            HttpContext.Request.Query.TryGetValue("name", out nameFilter);
            HttpContext.Request.Query.TryGetValue("rate", out rateFilter);

            string pName = nameFilter.ToString();
            Int32.TryParse(rateFilter.ToString(), out int pRate);

            var rooms = await _roomService.GetRoomsSimpleFilterAsync(pName, pRate);
            if (rooms == null)
                return NoContent();

            return Ok(rooms);
        }

        #endregion SimpleFiltering


        #region SimpleFilteringAttribute

        [HttpGet(Name = nameof(GetRoomsSimpleFilterAttribute))]
        public async Task<IActionResult> GetRoomsSimpleFilterAttribute([FromQuery] string name,
                                                                       [FromQuery] int rate)
        {
            var rooms = await _roomService.GetRoomsSimpleFilterAsync(name, rate);
            if (rooms == null)
                return NoContent();

            return Ok(rooms);
        }

        #endregion SimpleFilteringAttribute

        #region Filtering - Paging

        [HttpGet(Name = nameof(GetRoomsPaged))]
        public async Task<IActionResult> GetRoomsPaged([FromQuery] RoomFilter roomFilter)
        {
            // validasyon 
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // IQueryCollection queryString = Request.Query;
            // roomFilter.RouteValues = queryString.ToRouteValues();
            roomFilter.RouteName = string.Concat(Request.Scheme, "://", Request.Host.Value);
            roomFilter.RouteValues = Request.Query.ToRouteValues();

            var rooms = await _roomService.GetRoomsPagedAsync(roomFilter);

            if (rooms.Data == null || !rooms.Data.Any())  //rooms.Data.Count() <= 0 ) /* 1 2 3 4 ... 10.000*/
                return NoContent();

            return Ok(rooms);


        }

        #endregion Filtering - Paging
    }
}
