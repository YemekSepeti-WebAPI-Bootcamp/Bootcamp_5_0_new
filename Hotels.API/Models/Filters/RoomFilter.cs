using System.Xml.Serialization;
using System.Linq;
using System;
using System.ComponentModel.DataAnnotations;
using Hotels.API.Entities;

namespace Hotels.API.Models.Filters
{
    public sealed class RoomFilter : FilterBase
    {
        [Required]
        public string Name { get; set; }
        public int Rate { get; set; }

        public IQueryable<RoomEntity> ApplyTo(IQueryable<RoomEntity> roomQuery)
                    => roomQuery
                        .Where(room => room.Name.StartsWith(Name))
                        //.Where(room => room.Rate.Equals(Rate))
                        .Skip(base.SkipPage)
                        .Take(base.TakeCount)
                        .AsQueryable();

        public RoomFilter()
        {
            base.PageIndex = 1;
            base.RowsPerPage = 10;
        }

    }
}