using System;

namespace Hotels.API.Entities
{
    public class RoomEntity
    {
        public Guid Id {get;set;}
        public string Name { get; set; }
        
        public int Rate { get; set; }
        
        public bool IsMigrate { get; set; }

        
    }
}
