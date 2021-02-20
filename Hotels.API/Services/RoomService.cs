using System.Data;
using System.Runtime.CompilerServices;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Hotels.API.Contexts;
using Hotels.API.Models.Derived;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using System.Linq;
using Hotels.API.Models.Paging;
using Hotels.API.Models.Filters;
using Hotels.API.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Hotels.API.Caches;

namespace Hotels.API.Services
{
    public class RoomService : IRoomService
    {
        private readonly HotelApiDbContext _dbContext;
        private readonly IMapper _mapper;
        private readonly IUrlHelper _urlHelper;
        private readonly IMemoryCache _memCache;

        public RoomService(HotelApiDbContext dbContext, IMemoryCache memCache, IMapper mapper, IUrlHelper urlHelper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
            _urlHelper = urlHelper;
            _memCache = memCache;
        }

        #region OtherGets
        public async Task<List<Room>> GetRoomsAsync()
        {
            var roomEntities = await _dbContext.Rooms.ToListAsync();
            var result = roomEntities.Select(room => _mapper.Map<Room>(room))
                                     .ToList();

            return result;
        }
        public async Task<Room> GetRoomAsync(Guid id)
        {
            var roomEntity = await _dbContext.Rooms.SingleOrDefaultAsync(room => room.Id == id);
            if (roomEntity == null)
                return null;

            return _mapper.Map<Room>(roomEntity);


        }


        #endregion OtherGets

        #region SimpleFiltering

        public async Task<List<Room>> GetRoomsSimpleFilterAsync(string pName, int pRate)
        {
            var rooms = _dbContext.Rooms.AsQueryable();
            if (!string.IsNullOrWhiteSpace(pName))
                rooms = rooms.Where(room => room.Name.Contains(pName));

            if (pRate > 0)
                rooms = rooms.Where(room => room.Rate.Equals(pRate));

            /*
            Select [C1].Name, [C1].Rate From Rooms
             Where [C1].Name like '%Suid%'
               AND [C1].Rate = 34524;
            */

            var roomEntites = await rooms.ToListAsync();

            return roomEntites.Select(room => _mapper.Map<Room>(room)).ToList();

        }

        #endregion SimpleFiltering

        #region Filter-paging

        public async Task<PagedResponse<Room>> GetRoomsPagedAsync(RoomFilter filter)
        {

            var _key = $"name={filter.Name}&rate={filter.Rate}&pageindex={filter.PageIndex}&rowsperpage={filter.RowsPerPage}";

            List<RoomEntity> dbRecords = null;

            if (!_memCache.TryGetValue(CacheKeys.GetCacheKeyRoomList(_key), out dbRecords))
            {
                var cacheOptions = new MemoryCacheEntryOptions
                {
                    AbsoluteExpiration = DateTime.Now.AddMinutes(1),
                    // SlidingExpiration = DateTime.Now.AddMinutes(1)
                    Priority = CacheItemPriority.Normal
                };

                var entityData = await filter.ApplyTo(_dbContext.Rooms).ToListAsync();
                if (entityData.Any())
                {
                    dbRecords = entityData;
                    _memCache.Set(CacheKeys.GetCacheKeyRoomList(_key), dbRecords, cacheOptions);
                }
            }

            dbRecords = dbRecords ?? new List<RoomEntity>();

            var rooms = dbRecords.Select(room => _mapper.Map<Room>(room)).ToList();
            PagedResponse<Room> pageResponse = new PagedResponse<Room>
            {
                Data = rooms,
                Links = new Links
                {
                    NextPage = string.Concat(filter.RouteName, _urlHelper.RouteUrl(filter.NextRouteValues)),
                    PreviousePage = string.Concat(filter.RouteName, _urlHelper.RouteUrl(filter.PreviousRouteValues))
                }
            };

            return pageResponse;

            /*
            // Filtering Logic1
            // Func<RoomFilter, IQueryable<RoomEntity>> Filtering = (model) => 
            // {
            //     return _dbContext.Rooms.AsQueryable().Where(room => room.Name.StartsWith(model.Name));
            // };

            // var fiteredData = await Filtering(filter).ToListAsync();

            // Filtering Logic2
            var roomEntities = await filter.ApplyTo(_dbContext.Rooms).ToListAsync();
            var rooms = roomEntities.Select(room => _mapper.Map<Room>(room)).ToList();

            // Paging 
            PagedResponse<Room> pageResponse = new PagedResponse<Room>
            {
                Data = rooms,
                Links = new Links
                {
                    NextPage = string.Concat(filter.RouteName, _urlHelper.RouteUrl(filter.NextRouteValues)),
                    PreviousePage = string.Concat(filter.RouteName, _urlHelper.RouteUrl(filter.PreviousRouteValues))
                }
            };

            return pageResponse;
            */





        }

        #endregion Filter-paging
    }
}
