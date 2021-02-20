using System;
using Hotels.API.Entities;
using Microsoft.EntityFrameworkCore;

namespace Hotels.API.Contexts
{
    public class HotelApiDbContext : DbContext
    {
        public HotelApiDbContext(DbContextOptions options) : base(options)
        {
            
        }

        public DbSet<RoomEntity> Rooms {get;set;}
        public DbSet<UserEntity> Users {get;set;}

    }
}
